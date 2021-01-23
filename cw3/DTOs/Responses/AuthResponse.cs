﻿using cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Responses
{
    public class AuthResponse
    {
        public Boolean Authenticated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public string RefreshToken { get; set;  }
    }
}
