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
        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EntryDTO>> GetAll()
        {
            throw new NotImplementedException();
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

        public Task<EntryDTO> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Insert(EntryDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<EntryDTO> Update(EntryDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
