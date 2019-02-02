using dictionary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IEntryRepository
    {
        Task<IEnumerable<EntryDTO>> GetAllEntryForTitle(Guid Id);
        Task<RequestStatus> VotePlus(Guid Id,bool isVoted);
        Task<RequestStatus> CheckForVote(Guid model);
        Task<bool> AddToVoted(Guid UserId, Guid EntryId, bool vote);
        Task<bool> UpdateToVoted(Guid UserId, Guid EntryId, bool vote);
        void DeleteFromVoted(Guid Id);
        Task<RequestStatus> VoteMinus(Guid Id, bool isVoted);
    }
}
