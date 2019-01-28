using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class RequestStatus
    {
        [JsonProperty("s")]
        public bool Status { get; set; }
        [JsonProperty("sim")]
        public string StatusInfoMessage { get; set; }
    }
}
