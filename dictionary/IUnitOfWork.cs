using dictionary.Helpers;
using dictionary.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository _authRepository { get; set; }
        ITitleRepository _titleRepository { get; set; }
        RedisHandler _redisHandler { get; set; }

 
        IConfiguration _configuration { get; set; }

        void Commit();
    }
}
