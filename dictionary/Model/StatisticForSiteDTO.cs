using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class StatisticForSiteDTO : RequestStatus
    {
        [JsonProperty("ec")]
        public IEnumerable<int> EntryCount { get; set; }
        [JsonProperty("tc")]
        public IEnumerable<int> TitleCount { get; set; }
        [JsonProperty("uc")]
        public IEnumerable<int>  UserCount { get; set; }

        [JsonProperty("vc")]
        public IEnumerable<int> VoteCount { get; set; }
        [JsonProperty("tec")]
        public IEnumerable<int> TodayEntryCount { get; set; }
        [JsonProperty("yec")]
        public IEnumerable<int> YesterdayEntryCount { get; set; }
        [JsonProperty("lu")]
        public IEnumerable<UserDTO> LastUser { get; set; }

    }
}
