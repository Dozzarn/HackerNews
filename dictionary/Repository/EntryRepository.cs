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

        //TODO: AUTHORİZE 
        public async Task<RequestStatus> VoteMinus(Guid Id, bool isVoted)
        {
            var data = await GetByIdAsync(Id);

            if (data != null)
            {
                if (isVoted == true)
                    data.VotePlus -= 1;
                data.VoteMinus += 1;
                await UpdateAsync(data);
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

        public async Task<RequestStatus> VotePlus(Guid Id, bool isVoted)
        {
            var data = await GetByIdAsync(Id);

            if (data != null)
            {
                if (isVoted == true)
                    data.VoteMinus -= 1;
                data.VotePlus += 1;
                await UpdateAsync(data);
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
        public async Task<bool> AddToVoted(Guid UserId, Guid EntryId, bool vote)
        {
            var sql = "insert into [Voted] (UserId,EntryId,IsVotedPlus,IsVotedMinus) values(@ui,@ei,@ivp,@ivm)";
            var result = await Connection.ExecuteAsync(sql, new { ui = UserId, ei = EntryId, ivp = vote == true ? true : false, ivm = vote == true ? false : true }, transaction: Transaction);
            if (result != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        public async Task<bool> UpdateToVoted(Guid UserId, Guid EntryId, bool vote)
        {
            var sql = "update [Voted] set  IsVotedPlus=@ivp,IsVotedMinus=@ivm where UserId=@ui and EntryId=@ei";
            var result = await Connection.ExecuteAsync(sql, new { ivp = vote == true ? true : false, ivm = vote == true ? false : true,ui = UserId,ei = EntryId }, transaction: Transaction);
            if (result != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }
        public async Task<RequestStatus> CheckForVote(Guid Id)
        {
            var sql = "select * from [Voted] where EntryId=@ei";
            var data = await Connection.QueryFirstOrDefaultAsync<VotedDTO>(sql, new { ei = Id }, transaction: Transaction);
            if (data != null)
            {
                if (data.IsVotedMinus == true)
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = true,
                        StatusInfoMessage = "Eksi"
                    });
                }
                else if (data.IsVotedPlus == true)
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = true,
                        StatusInfoMessage = "Artı"
                    });
                }

            }
            return await Task.FromResult(new RequestStatus
            {
                Status = true,
                StatusInfoMessage = "Boş"
            });
        }

        #region CRUD
        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "delete from [Entry] where EntryId=@ei";

            var data = await Connection.ExecuteAsync(sql, new { ei = id }, transaction: Transaction);
            if (data != 0)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public Task<IEnumerable<EntryDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }



        public async Task<EntryDTO> GetByIdAsync(Guid id)
        {
            var sql = "select * from [Entry] where EntryId=@Id";

            var data = await Connection.QueryFirstOrDefaultAsync<EntryDTO>(sql, new { Id = id }, transaction: Transaction);

            if (data != null)
            {

                return await Task.FromResult(data);
            }
            return null;
        }

        public Task<bool> InsertAsync(EntryDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<EntryDTO> UpdateAsync(EntryDTO model)
        {
            var sql = "update [Entry] set Entry=@e,UserId=@user,Time=@time,VoteMinus=@minus,VotePlus=@plus where EntryId=@ei";

            var updatedModel = await Connection.ExecuteAsync(sql, new { e = model.Entry, User = model.UserId, time = model.Time, minus = model.VoteMinus, plus = model.VotePlus, ei = model.EntryId }, transaction: Transaction);

            if (updatedModel != 0)
            {
                return await Task.FromResult(model);
            }
            return null;
        }

        public IEnumerable<EntryDTO> GetAll()
        {
            var sql = "select * from [Entry]";

            var data = Connection.Query<EntryDTO>(sql, transaction: Transaction);

            if (data != null)
            {
                return data;
            }
            return null;
        }

        public EntryDTO Update(EntryDTO model)
        {
            throw new NotImplementedException();
        }

        public bool Insert(EntryDTO model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public EntryDTO GetById(Guid id)
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}
