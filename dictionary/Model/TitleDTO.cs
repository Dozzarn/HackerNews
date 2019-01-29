using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleDTO
    {
        [JsonProperty("ti")]
        public Guid TitleId { get; set; }
        [JsonProperty("t")]
        public string Title { get; set; }
        [JsonProperty("ui")]
        public Guid UserId { get; set; }
        [JsonProperty("u")]
        public string Username { get; set; }

        [JsonProperty("ei")]
        public Guid EntryId { get; set; }
        [JsonProperty("e")]
        public string Entry { get; set; }
        [JsonProperty("d")]
        public DateTime Time { get; set; }
        [JsonProperty("c")]
        public string Category { get; set; }
        [JsonProperty("vm")]
        public int VoteMinus { get; set; }
        [JsonProperty("vp")]
        public int VotePlus { get; set; }
        [JsonProperty("ve")]
        public int TotalEntry { get; set; }

    }
}
