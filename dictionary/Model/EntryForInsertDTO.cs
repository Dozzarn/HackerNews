using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class EntryForInsertDTO
    {

        [JsonProperty("e")]
        public string Entry { get; set; } 
        [JsonProperty("ti")]
        public Guid TitleId { get; set; }
    }
}
