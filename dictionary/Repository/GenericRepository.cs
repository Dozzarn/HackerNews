using Dapper;
using dictionary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public class GenericRepository<T> : BaseRepository, IGenericRepository<T> where T : class
    {
        public GenericRepository(IDbTransaction transaction) : base(transaction)
        {

        }

        public bool Delete(string sql, object param)
        {
            //var sql = "delete from [Entry] where EntryId=@ei";

            var data =  Connection.Execute(sql, param, transaction: Transaction);
            if (data != 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(string sql, object param)
        {
            //var sql = "delete from [Entry] where EntryId=@ei";

            var data = await Connection.ExecuteAsync(sql, param, transaction: Transaction);
            if (data != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public IEnumerable<T> GetAll(string sql)
        {

            var data = Connection.Query<T>(sql, transaction: Transaction);

            if (data != null)
            {
                return data ;
            }
            return null;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string sql)
        {
            //var sql = "select * from [Entry]";

            var data = await Connection.QueryAsync<T>(sql, transaction: Transaction);

            if (data != null)
            {
                return data;
            }
            return null;
        }

        public T GetById(string sql, object param)
        {
            //var sql = "select * from [Entry] where EntryId=@Id";

            var data =  Connection.QueryFirstOrDefault<T>(sql, param, transaction: Transaction);

            if (data != null)
            {

                return data;
            }
            return null;
        }

        public async Task<T> GetByIdAsync(string sql, object param)
        {
            //var sql = "select * from [Entry] where EntryId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction: Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;
        }

        public bool Insert(string sql, object param)
        {
            var insertedModel =  Connection.Execute(sql, param, transaction: Transaction);

            if (insertedModel != 0)
            {
                return true;
            }
            return  false;
        }

        public async Task<bool> InsertAsync(string sql, object param)
        {
            var insertedModel = await Connection.ExecuteAsync(sql, param, transaction: Transaction);

            if (insertedModel != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public bool Update(string sql, object param)
        {
            var updatedModel =  Connection.Execute(sql, param, transaction: Transaction);

            if (updatedModel != 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(string sql, object param)
        {
            //var sql = "update [Entry] set Entry=@e,UserId=@user,Time=@time,VoteMinus=@minus,VotePlus=@plus where EntryId=@ei";

            var updatedModel = await Connection.ExecuteAsync(sql,param, transaction: Transaction);

            if (updatedModel != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }
    }
}
