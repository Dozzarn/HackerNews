using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TotalActivityDTO :RequestStatus
    {
       [JsonProperty("el")]
        public IEnumerable<EntryDTO> EntryList { get; set; }
        [JsonProperty("tp")]
        public IEnumerable<int> TotalPlus { get; set; }
        [JsonProperty("tm")]
        public IEnumerable<int> TotalMinus { get; set; }
        [JsonProperty("tl")]
        public IEnumerable<TitleDTO> TitleList { get; set; }
    }

}
