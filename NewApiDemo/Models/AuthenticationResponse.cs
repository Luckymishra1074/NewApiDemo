using NewApiDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }

        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }

        public string UserName { get; set; }
     

        public AuthenticationResponse(User user,string Token)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserName = user.Username;
            this.Token = Token;

        }
    }
}
