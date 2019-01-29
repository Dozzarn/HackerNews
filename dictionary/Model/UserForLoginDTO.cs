using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class UserForLoginDTO
    {
        [JsonProperty("u")]
        public string Username { get; set; }
        [JsonProperty("p")]
        public string Password { get; set; }
        [JsonProperty("rm")]
        public bool RememberMe { get; set; }

    }
}
