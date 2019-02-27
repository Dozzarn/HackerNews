using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleForSearchDTO
    {
        [JsonProperty("ti")]
        public Guid TitleId { get; set; }
        [JsonProperty("t")]
        public string Title { get; set; }
    }
}
