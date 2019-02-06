using dictionary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IHelperRepository
    {
        Task<SearchForRequestDTO> Search(SearchDTO model);
    }
}
