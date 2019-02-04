using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Model;

namespace dictionary.Repository
{
    public class EntryRepository : GenericRepository<EntryDTO>, IEntryRepository
    {

        public EntryRepository(IDbTransaction transaction) : base(transaction)
        {

        }

        //TODO: AUTHORİZE 
        public async Task<RequestStatus> VoteMinus(Guid Id, bool isVoted)
        {
            var sql = "select * from [Entry] where EntryId=@id";
            var param = new { id = Id };
            var model = await GetByIdAsync(sql, param);
            if (model != null)
            {
                if (isVoted == true)
                    model.VotePlus -= 1;
                model.VoteMinus += 1;
                sql = "update [Entry] set Entry=@e,UserId=@user,Time=@time,VoteMinus=@minus,VotePlus=@plus where EntryId=@ei";
                var param2 = new { e = model.Entry, User = model.UserId, time = model.Time, minus = model.VoteMinus, plus = model.VotePlus, ei = model.EntryId };
                await UpdateAsync(sql, param2);
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
            var sql = "select * from [Entry] where EntryId=@id";
            var param = new { id = Id };
            var model = await GetByIdAsync(sql, param);

            if (model != null)
            {
                if (isVoted == true)
                    model.VoteMinus -= 1;
                model.VotePlus += 1;
                sql = "update [Entry] set Entry=@e,UserId=@user,Time=@time,VoteMinus=@minus,VotePlus=@plus where EntryId=@ei";
                var param2 = new { e = model.Entry, User = model.UserId, time = model.Time, minus = model.VoteMinus, plus = model.VotePlus, ei = model.EntryId };
                await UpdateAsync(sql, param2);
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

        public async void DeleteFromVoted(Guid Id)
        {
            var sql = "delete from [Voted] where EntryId=@ei";
            var param = new { ei = Id };
            var data = await DeleteAsync(sql,param);
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
            var param = new { ui = UserId, ei = EntryId, ivp = vote == true ? true : false, ivm = vote == true ? false : true };
            var result = await InsertAsync(sql,param);
            if (result != false)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        public async Task<bool> UpdateToVoted(Guid UserId, Guid EntryId, bool vote)
        {
            var sql = "update [Voted] set  IsVotedPlus=@ivp,IsVotedMinus=@ivm where UserId=@ui and EntryId=@ei";
            var param = new { ivp = vote == true ? true : false, ivm = vote == true ? false : true, ui = UserId, ei = EntryId };
            var result = await UpdateAsync(sql, param);
            if (result != false)
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



    }
}
