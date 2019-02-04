using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class EntryForUpdateResultDTO:RequestStatus
    {
        [JsonProperty("e")]
        public string Entry { get; set; }
    }
}
