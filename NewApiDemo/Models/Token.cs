using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class Token
    {
        [Key]
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }
}
