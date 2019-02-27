using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class SearchDTO
    {
        [JsonProperty("t")]
        public string Text { get; set; }
        
    }
}
