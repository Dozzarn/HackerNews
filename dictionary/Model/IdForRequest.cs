using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class IdForRequest:RequestStatus
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }
    }
}
