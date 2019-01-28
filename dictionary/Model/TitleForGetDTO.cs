using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleForGetDTO :RequestStatus
    {
        [JsonProperty("t")]
        public TitleDTO Title { get; set; }
        public IEnumerable<EntryDTO> Entries { get; set; }

    }
}
