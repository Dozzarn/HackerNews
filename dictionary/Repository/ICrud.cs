using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface ICrud<T>   where T : class 
    {
        Task<T> UpdateAsync(T model);
        Task<bool> InsertAsync(T model);
        Task<bool> DeleteAsync(Guid id);
        T Update(T model);
        bool Insert(T model);
        bool Delete(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        T GetById(Guid id);

        Task<T> GetByIdAsync(Guid id);
    }
}
