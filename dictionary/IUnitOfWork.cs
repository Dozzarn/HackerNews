using dictionary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary
{
    interface IUnitOfWork : IDisposable
    {

        void Commit();
    }
}
