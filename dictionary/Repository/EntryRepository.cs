using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Model;

namespace dictionary.Repository
{
    public class EntryRepository : BaseRepository, IEntryRepository
    {
        public EntryRepository(IDbTransaction transaction) : base(transaction)
        {

        }


        public async Task<RequestStatus> VoteMinus(Guid Id)
        {
            var data = await GetById(Id);

            if (data != null)
            {
                data.VoteMinus += 1;
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

        public async Task<IEnumerable<EntryDTO>> GetAllEntryForTitle(Guid Id)
        {
            var sql = "select b.*,a.Username from [Entry] b inner join [User] a on b.UserId=a.Id where TitleId=@id";

            var data = await Connection.QueryAsync<EntryDTO>(sql, new { id = Id }, transaction: Transaction);
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

        public Task<IEnumerable<EntryDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        

        public async Task<EntryDTO> GetById(Guid id)
        {
            var sql = "select * from [Entry] where EntryId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<EntryDTO>(sql, new { Id = id }, transaction: Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;
        }

        public Task<bool> Insert(EntryDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<EntryDTO> Update(EntryDTO model)
        {
            var sql = "update [Entry] set Entry=@e,UserId=@user,Time=@time,VoteMinus=@minus,VotePlus=@plus where EntryId=@ei";

            var updatedModel = await Connection.ExecuteAsync(sql, new { e = model.Entry, User = model.UserId, time = model.Time, minus = model.VoteMinus, plus = model.VotePlus, ei = model.EntryId }, transaction: Transaction);

            if (updatedModel != 0)
            {
                return await Task.FromResult(model);
            }
            return null;
        }
        #endregion
    }
}
