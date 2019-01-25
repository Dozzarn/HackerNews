using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Helpers;
using dictionary.Model;

namespace dictionary.Repository
{
    public class AuthRepository : BaseRepository, IAuthRepository
    {
        private readonly IGenericRepository<User> _genericRepository;
        private RedisHandler _redis;
        public AuthRepository(IDbTransaction transaction) : base(transaction)
        {
            _redis = new RedisHandler();
            _genericRepository = new GenericRepository<User>(transaction);
        }

        private async Task<User> IsUserExists(User user)
        {

            var sql = $"select * from [user] where Username=@Search";
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

            await _genericRepository.Insert(user);

            return await Task.FromResult(user);
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
    }
}
