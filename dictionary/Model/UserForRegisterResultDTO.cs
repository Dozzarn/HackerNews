using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class UserForRegisterResultDTO : RequestStatus
    {

        [JsonProperty("u")]
        public string Username { get; set; }
        [JsonProperty("e")]
        public string Email { get; set; }
    }
}
