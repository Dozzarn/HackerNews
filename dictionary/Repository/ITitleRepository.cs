using dictionary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface ITitleRepository : ICrud<TitleDTO>
    {
        Task<TitleDTO> IsBinded(Guid Id);
        
    }
}
