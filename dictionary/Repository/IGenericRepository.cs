using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IGenericRepository<T> where T : class
    {

        //TODO: GENERİC CRUD IMPLEMENTATION
        Task<bool> UpdateAsync(string sql,object param);
        Task<bool> InsertAsync(string sql, object param);
        Task<bool> DeleteAsync(string sql, object param);
        bool Update(string sql, object param);
        bool Insert(string sql, object param);
        bool Delete(string sql, object param);
        Task<IEnumerable<T>> GetAllAsync(string sql);
        IEnumerable<T> GetAll(string sql);
        T GetById(string sql, object param);

        Task<T> GetByIdAsync(string sql, object param);
    }
}
