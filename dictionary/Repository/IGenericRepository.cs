using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IGenericRepository<T> : IDisposable  where T : class 
    {
        Task<T> Update(T model);
        Task<bool> Insert(T model);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(Guid id);

        IDbTransaction _transaction { get; set; }
    }
}
