﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class UserForRegisterDTO
    {
        [JsonProperty("u")]
        public string Username { get; set; }
        [JsonProperty("e")]
        public string Email { get; set; }
        [JsonProperty("p")]
        public string Password { get; set; }
        [JsonProperty("pa")]
        public string PasswordAgain { get; set; }
    }
}
