using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewApiDemo.Models
{
    public class Product
    {
           [Required(ErrorMessage ="UserName is Required")]
        public string Username { get; set; }
    }
}
