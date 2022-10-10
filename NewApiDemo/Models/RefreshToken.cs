﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshTokens { get; set; }

        public string UserName { get; set; }

    }
}
