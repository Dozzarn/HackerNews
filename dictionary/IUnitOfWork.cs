using dictionary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository _authRepository { get; set; }
        void Commit();
    }
}
