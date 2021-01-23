using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        public string IndexNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
