using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}
