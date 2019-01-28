using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class EntryDTO
    {
        [JsonProperty("ei")]
        public Guid EntryId { get; set; }
        [JsonProperty("e")]

        public string Entry { get; set; }
        [JsonProperty("d")]

        public DateTime Time { get; set; }

        [JsonProperty("u")]

        public string Username { get; set; }

        [JsonProperty("ti")]

        public Guid TitleId { get; set; }
        [JsonProperty("vm")]

        public int VoteMinus { get; set; }
        [JsonProperty("vp")]

        public int VotePlus { get; set; }
    }
}
