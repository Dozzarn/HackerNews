using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }

        public string Email {get;set;}
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }


    }
}
