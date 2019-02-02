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
    public class TitleRepository : GenericRepository<TitleDTO>, ITitleRepository
    {

        public TitleRepository(IDbTransaction transaction) : base(transaction)
        {
        }


        public async Task<TitleDTO> IsBinded(Guid Id)
        {
            var sql = "select * from [Title] where EntryId=@id";
            var param = new { id = Id };
            var data = await GetByIdAsync(sql, param);
            if (data != null)
            {
                return await Task.FromResult(data);
            }
            return null;

        }
        private async Task<TitleDTO> IsTitleExists(TitleDTO title)
        {

            var sql = $"select * from [Title] where Title=@data";
            var param = new { t = title.Title };
            var data = await GetByIdAsync(sql, param);
            if (data != null)
            {
                return await Task.FromResult(data);
            }

            return null;


        }

   

    }
}
