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

        public TitleRepository(IDbTransaction transaction) : base(transaction)
        {
        }


        public async Task<TitleDTO> IsBinded(Guid Id)
        {
            var sql = "select * from [Title] where EntryId=@id";
            var data = await Connection.QueryFirstOrDefaultAsync<TitleDTO>(sql,new { id=Id },transaction:Transaction);
            if (data != null)
            {
                return await Task.FromResult(data);
            }
            return null;

        }
        private async Task<TitleDTO> IsTitleExists(TitleDTO title)
        {

            var sql = $"select * from [Title] where Title=@data";
            var data = await Connection.QueryFirstOrDefaultAsync<TitleDTO>(sql, new { data = title.Title }, transaction: Transaction);
            if (data != null)
            {
                return await Task.FromResult(data);
            }

            return null;


        }

        #region CRUD


        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TitleDTO>> GetAllAsync()
        {
            var sql = "select a.*,b.Entry from [Title] a inner join [Entry] b on a.EntryId=b.EntryId";

            var data = await Connection.QueryAsync<TitleDTO>(sql, transaction: Transaction);

            if (data != null)
            {
                return await Task.FromResult(data);
            }
            return null;
        }

        public async Task<TitleDTO> GetByIdAsync(Guid id)
        {
            var sql = "select * from [Title] where TitleId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<TitleDTO>(sql, new { Id = id }, transaction: Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;

        }

        public async Task<bool> InsertAsync(TitleDTO model)
        {
            var sql = "insert into [Title] (Title,Username,Time,Category) values (@title,@user,@time,@cat)";

            var insertedModel = await Connection.ExecuteAsync(sql, new { title = model.Title, user = model.Username, time = model.Time, cat = model.Category }, transaction: Transaction);

            if (insertedModel != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public async Task<TitleDTO> UpdateAsync(TitleDTO model)
        {
            var sql = "update [Title] set Title=@title,UserId=@ui,Time=@time,Category=@cat,VoteMinus=@minus,VotePlus=@plus,EntryId=@ei where TitleId=@id";
            
            var updatedModel = await Connection.ExecuteAsync(sql, new { title = model.Title, ui = model.UserId, time = model.Time, cat = model.Category,ei=model.EntryId, minus = model.VoteMinus, plus = model.VotePlus,id=model.TitleId },transaction:Transaction);

            if (updatedModel != 0)
            {
                return await Task.FromResult(model);
            }
            return  null;

        }

        public IEnumerable<TitleDTO> GetAll()
        {
            var sql = "select a.*,b.Entry from [Title] a inner join [Entry] b on a.EntryId=b.EntryId";

            var data =  Connection.Query<TitleDTO>(sql, transaction: Transaction);

            if (data != null)
            {
                return data;
            }
            return null;
        }

        public TitleDTO Update(TitleDTO model)
        {
            throw new NotImplementedException();
        }

        public bool Insert(TitleDTO model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public TitleDTO GetById(Guid id)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
