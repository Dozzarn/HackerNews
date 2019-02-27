using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class UserForSearchDTO
    {
        [JsonProperty("ui")]
        public Guid Id { get; set; }
        [JsonProperty("u")]
        public string Username { get; set; }
    }
}
