using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class VotedDTO:RequestStatus
    {
        [JsonProperty("ui")]
        public Guid UserId { get; set; }
        [JsonProperty("ei")]
        public Guid EntryId { get; set; }
        [JsonProperty("isp")]
        public bool IsVotedPlus { get; set; }
        [JsonProperty("ism")]
        public bool IsVotedMinus { get; set; }
    }
}
