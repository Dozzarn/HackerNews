using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Model;

namespace dictionary.Repository
{
    public class DbRepository<T> : IDbRepository<T> where T : class
    {

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

        public async Task<T> GetByModel(T model, string findFor)
        {

            var type = model.GetType();
            var search = type.GetProperties().FirstOrDefault(x => x.Name == findFor);

            using (var conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dictionary;Integrated Security=True"))
            {
                var sql = $"select * from [user] where {findFor}=@Search";

                var data = await conn.QueryFirstOrDefaultAsync<User>(sql, new { Search = search });
                if (data != null)
                {
                    return await Task.FromResult(data as T);
                }
                return null;
            }

        }

        public async Task<bool> Insert(T model)
        {
            var type = model.GetType();
            var obj = type.GetProperties();

            using (var conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dictionary;Integrated Security=True"))
            {
                var sql = "insert into user (Username,PasswordHash,PasswordSalt) values (@name,@hash,@salt)";

                var data = await conn.ExecuteAsync(sql, new { name = obj.GetValue(1), hash = obj.GetValue(2), salt = obj.GetValue(3) });
                if (data != 0)
                {
                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            }
        }

        public Task<T> Update(T model)
        {
            throw new NotImplementedException();
        }
    }
}
