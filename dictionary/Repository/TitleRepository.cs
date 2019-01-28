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


        public async Task<RequestStatus> VoteMinus(Guid Id)
        {
            var data = await GetById(Id);

            if (data != null)
            {
                data.VotePlus -= 1;
                await Update(data);
                return await Task.FromResult(new RequestStatus
                {
                    Status = true,
                    StatusInfoMessage = "Başarılı"
                });
            }
            return await Task.FromResult(new RequestStatus
            {
                Status = false,
                StatusInfoMessage = "Başarısız"
            });
        }

        public async Task<RequestStatus> VotePlus(Guid Id)
        {
            var data = await GetById(Id);

            if (data != null)
            {
                data.VotePlus += 1;
                await Update(data);
                return await Task.FromResult(new RequestStatus
                {
                    Status = true,
                    StatusInfoMessage = "Başarılı"
                });
            }
            return await Task.FromResult(new RequestStatus
            {
                Status = false,
                StatusInfoMessage = "Başarısız"
            });

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


        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TitleDTO>> GetAll()
        {
            var sql = "select a.*,b.Entry from [Title] a inner join [Entry] b on a.EntryId=b.EntryId";

            var data = await Connection.QueryAsync<TitleDTO>(sql, transaction: Transaction);

            if (data != null)
            {
                return await Task.FromResult(data);
            }
            return null;
        }

        public async Task<TitleDTO> GetById(Guid id)
        {
            var sql = "select * from [Title] where TitleId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<TitleDTO>(sql, new { Id = id }, transaction: Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;

        }

        public async Task<bool> Insert(TitleDTO model)
        {
            var sql = "insert into [Title] (Title,Username,Time,Category) values (@title,@user,@time,@cat)";

            var insertedModel = await Connection.ExecuteAsync(sql, new { title = model.Title, user = model.Username, time = model.Time, cat = model.Category }, transaction: Transaction);

            if (insertedModel != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public async Task<TitleDTO> Update(TitleDTO model)
        {
            var sql = "update [Title] set Title=@title,Username=@user,Time=@time,Category=@cat,VoteMinus=@minus,VotePlus=@plus";

            var updatedModel = await Connection.ExecuteAsync(sql, new { title = model.Title, user = model.Username, time = model.Time, cat = model.Category, minus = model.VoteMinus, plus = model.VotePlus });

            if (updatedModel != 0)
            {
                return await Task.FromResult(model);
            }
            return  null;

        }
        #endregion

    }
}
