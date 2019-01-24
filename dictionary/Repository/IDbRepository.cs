using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IDbRepository<T> where T : class
    {
        Task<T> Update(T model);
        Task<bool> Insert(T model);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(Guid id);

        Task<T> GetByModel(T model, string findFor);


    }
}
