using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class Product
    {
           [Required(ErrorMessage ="UserName is Required")]
        public string Username { get; set; }
    }
}
