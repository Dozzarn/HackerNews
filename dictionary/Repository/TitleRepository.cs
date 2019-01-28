using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Helpers;
using dictionary.Model;
using Dapper;
namespace dictionary.Repository
{
    public class TitleRepository : BaseRepository, ITitleRepository
    {
        private RedisHandler _redis;

        public TitleRepository(IDbTransaction transaction) : base(transaction)
        {
            _redis = new RedisHandler();
        }

       
        public Task VoteMinus(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task VotePlus(Guid Id)
        {
            throw new NotImplementedException();
        }






        #region CRUD
            

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TitleDTO>> GetAll()
        {
            var sql = "select * from Title";

            var data = await Connection.QueryAsync<TitleDTO>(sql,transaction:Transaction);

            if (data != null)
            {
                return await Task.FromResult(data);
            }
            return null;
        }

        public async Task<TitleDTO> GetById(Guid id)
        {
            var sql = "select * from Title where TitleId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<TitleDTO>(sql,new { Id=id},transaction:Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;

        }

        public async Task<bool> Insert(TitleDTO model)
        {
            var sql = "insert into Title (Title,UserId,Time,Category) values (@title,@user,@time,@cat)";

            var insertedModel = await Connection.ExecuteAsync(sql, new { title = model.Title, user = model.UserId, time = model.Time, cat = model.Category }, transaction: Transaction);

            if (insertedModel != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public Task<TitleDTO> Update(TitleDTO model)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
