using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

    }
}
