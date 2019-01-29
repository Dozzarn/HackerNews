using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class UserForLoginResultDTO:RequestStatus
    {
        [JsonProperty("t")]
        public string Token { get; set; }
    }
}
