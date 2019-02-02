using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleForInsertDTO
    {
        [JsonProperty("t")]
        public string Title { get; set; }
        [JsonProperty("c")]
        public string Category { get; set; }
        [JsonProperty("e")]
        public string Entry { get; set; }
    }
}
