﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using dictionary.Helpers;
using dictionary.Model;

namespace dictionary.Repository
{
    public class AuthRepository : GenericRepository<UserDTO>, IAuthRepository
    {

        public AuthRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        private async Task<UserDTO> IsUserExists(UserDTO user)
        {

            var sql = $"select * from [User] where Username=@Search";
            //var data = await Connection.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Search = user.Username }, transaction: Transaction);
            var data = await GetByIdAsync(sql, new { Search = user.Username });
            if (data != null)
            {
                return await Task.FromResult(data);
            }

            return null;


        }
        public async Task<UserDTO> Login(string userName, string password)
        {
            var model = new UserDTO
            {
                Username = userName
            };
            var user = await IsUserExists(model);
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return await Task.FromResult(user);
        }

        public async Task<UserDTO> Register(UserDTO user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var sql = "insert into [User] (Username,PasswordHash,PasswordSalt) values (@name,@hash,@salt)";
            var param = new { name = user.Username, hash = user.PasswordHash, salt = user.PasswordSalt };

            if (await InsertAsync(sql,param))
            {
                return await Task.FromResult(user);
            }

            return null;
        }

        public async Task<bool> UserExits(string userName)
        {
            UserDTO user = new UserDTO
            {
                Username = userName
            };

            var data = await IsUserExists(user);
            if (data != null)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }


            }
            return true;
        }

        public async Task<TotalActivityDTO> GetTotals(Guid UserId)
        {
            var sql1 = "select * from [Entry] a inner join [User] b on a.UserId=b.Id where UserId=@id";//total entry
            var sql2 = "select * from [Title] a inner join [User] b on a.UserId=b.Id inner join [Entry] c on a.EntryId=c.EntryId where a.UserId=@id";//total title
            var sql3 = "select count(*) from [Voted] where UserId=@id and IsVotedMinus=@ivm";//total minus
            var sql4 = "select count(*) from [Voted] where UserId=@id and IsVotedPlus=@ivp";//total plus
            var e = await Connection.QueryAsync<EntryDTO>(sql1, new { id = UserId }, transaction: Transaction);
            var t = await Connection.QueryAsync<TitleDTO>(sql2, new { id = UserId }, transaction: Transaction);
            var tm = await Connection.QueryAsync<int>(sql3, new { id = UserId,ivm=true}, transaction: Transaction);
            var tp = await Connection.QueryAsync<int>(sql4, new { id = UserId,ivp=true }, transaction: Transaction);
            return await Task.FromResult(new TotalActivityDTO
            {
                EntryList = e,
                TotalMinus = tm,
                TotalPlus = tp,
                TitleList = t,
                Status=true,
                StatusInfoMessage="Başarılı"
            });

        }



    }
}
