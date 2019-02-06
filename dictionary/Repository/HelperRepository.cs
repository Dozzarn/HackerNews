﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using Dapper;
namespace dictionary.Repository
{
    public class HelperRepository : GenericRepository<SearchDTO>, IHelperRepository 
    {
        public HelperRepository(IDbTransaction transaction): base(transaction)
        {

        }
        public async Task<SearchForRequestDTO> Search(SearchDTO model)
        {

            if (!string.IsNullOrEmpty(model.Text))
            {
                var text = model.Text;
                SearchForRequestDTO data = new SearchForRequestDTO();   
                if (text.Contains("@"))
                {
                    var sql = "select * from [User]";
                    var r  = await Connection.QueryAsync<UserForSearchDTO>(sql,transaction:Transaction);
                    data.Users = r.Where(x => x.Username.ToLower().Contains(text.Replace("@", string.Empty).ToLower())).Take(8);
                }
                else if (text.Contains("#"))
                {
                    //TODO: if entry null throwing exception
                    var sql = "select * from [Entry]";
                    var r = await Connection.QueryAsync<EntryForSearchDTO>(sql, transaction: Transaction);
                    data.Entries = r.Where(x => x.Entry.ToLower().Contains(text.Replace("#", string.Empty).ToLower())).Take(8);
                }
                else
                {
                    var sql = "select * from [Title] ";
                     var r= await Connection.QueryAsync<TitleForSearchDTO>(sql, transaction: Transaction);
                    data.Titles = r.Where(x => x.Title.ToLower().Contains(text.ToLower())).Take(8);
                }
                return await Task.FromResult(data);
            }
            else
            {
                return null;

            }
        }
    }
}
