using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using dictionary.Helpers;
using dictionary.Model;

namespace dictionary.Repository
{
    public class AuthRepository : BaseRepository, IAuthRepository, ICrud<User>
    {
        private RedisHandler _redis;

        public AuthRepository(IDbTransaction transaction) : base(transaction)
        {
            _redis = new RedisHandler();
        }

        private async Task<User> IsUserExists(User user)
        {

            var sql = $"select * from [User] where Username=@Search";
            var data = await Connection.QueryFirstOrDefaultAsync<User>(sql, new { Search = user.Username }, transaction: Transaction);
            if (data != null)
            {
                return await Task.FromResult(data);
            }

            return null;


        }
        public async Task<User> Login(string userName, string password)
        {
            var model = new User
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

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (await Insert(user))
            {
                return await Task.FromResult(user);
            }

            return null;
        }

        public async Task<bool> UserExits(string userName)
        {
            User user = new User
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

        

        #region CRUD
        public async Task<User> Update(User model)
        {
            var sql = "update [User] set Username=@name,PasswordHash=@hash,PasswordSalt=@salt where Id=@id";

            var data = await Connection.ExecuteAsync(sql, new { name = model.Username, hash = model.PasswordHash, salt = model.PasswordSalt,Id = model.Id }, transaction: Transaction);
            if (data != 0)
            {

                return await Task.FromResult(model);
            }

            return null;
        }

        public async Task<bool> Insert(User model)
        {
            var sql = "insert into [User] (Username,PasswordHash,PasswordSalt) values (@name,@hash,@salt)";

            var data = await Connection.ExecuteAsync(sql, new { name = model.Username, hash = model.PasswordHash, salt = model.PasswordSalt }, transaction: Transaction);
            if (data != 0)
            {
                
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var sql = "select * from [User]";

            var data = await Connection.QueryAsync<User>(sql, transaction: Transaction);
            if (data != null)
            {

                return await Task.FromResult(data);
            }

            return await Task.FromResult(data);
        }

        public async Task<User> GetById(Guid id)
        {
            var sql = "select * from [User] where Id=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<User>(sql,new { Id=id }, transaction: Transaction);
            if (data != null)
            {
                return await Task.FromResult(data);
            }

            return await Task.FromResult(data);
        }


        #endregion
    }
}
