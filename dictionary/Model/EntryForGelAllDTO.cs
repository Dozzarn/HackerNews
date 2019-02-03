using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class EntryForGelAllDTO:RequestStatus
    {
        [JsonProperty("ae")]
        public IEnumerable<EntryDTO> AllEntry { get; set; }
    }
}
