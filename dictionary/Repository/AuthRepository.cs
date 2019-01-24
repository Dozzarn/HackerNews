using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;

namespace dictionary.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbRepository<User> _dbRepository;
        public AuthRepository(IDbRepository<User> dbRepository)
        {
            _dbRepository = dbRepository;
        }
        public async Task<User> Login(string userName, string password)
        {
            var model = new User
            {
                Username = userName
            };
            var user = await _dbRepository.GetByModel(model,"Username");
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

            await _dbRepository.Insert(user);

            return await Task.FromResult(user);
        }

        public async Task<bool> UserExits(string userName)
        {
            User user = new User
            {
                Username = userName
            };

            var data = await _dbRepository.GetByModel(user,"Username");
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
