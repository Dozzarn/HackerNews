using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Model;

namespace dictionary.Repository
{
    public class GenericRepository<T> : BaseRepository,IGenericRepository<T> where T : class
    {

        public GenericRepository(IDbTransaction transaction): base(transaction)
        {
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(T model)
        {
            var type = model.GetType();
            var obj = type.GetProperties();

            var sql = "insert into user (Username,PasswordHash,PasswordSalt) values (@name,@hash,@salt)";
 
            var data = await Connection.ExecuteAsync(sql, new { name = obj.GetValue(1), hash = obj.GetValue(2), salt = obj.GetValue(3) },transaction:Transaction);
            if (data != 0)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public Task<T> Update(T model)
        {
            throw new NotImplementedException();
        }


    }
}
