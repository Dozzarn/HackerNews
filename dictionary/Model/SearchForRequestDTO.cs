using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class SearchForRequestDTO :RequestStatus 
    {
        [JsonProperty("t")]
        public IEnumerable<TitleForSearchDTO> Titles { get; set; }
        [JsonProperty("e")]
        public IEnumerable<EntryForSearchDTO> Entries { get; set; }
        [JsonProperty("u")]
        public IEnumerable<UserForSearchDTO> Users { get; set; }
    }
}
