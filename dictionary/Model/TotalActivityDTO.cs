using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TotalActivityDTO :RequestStatus
    {
           [JsonProperty("te")]
        public IEnumerable<int> TotalEntry { get; set; }
        [JsonProperty("tp")]
        public IEnumerable<int> TotalPlus { get; set; }
        [JsonProperty("tm")]
        public IEnumerable<int> TotalMinus { get; set; }
        [JsonProperty("tt")]
        public IEnumerable<int> TotalTitle { get; set; }
    }

}
