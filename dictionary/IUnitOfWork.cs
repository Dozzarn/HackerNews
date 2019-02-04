using dictionary.Helpers;
using dictionary.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dictionary
{
    public interface IUnitOfWork<T> : IDisposable  where T:class
    { 
        IAuthRepository _authRepository { get; set; }
        ITitleRepository _titleRepository { get; set; }
        JwtSecurityTokenHandler _tokenHandler { get; set; }
        IEntryRepository _entryRepository { get; set; }
        IGenericRepository<T> _genericRepository { get; set; }
        RedisHandler _redisHandler { get; set; }
        IConfiguration _configuration { get; set; }




        JwtSecurityToken getToken(StringValues token);
        void Commit();
    }
}
