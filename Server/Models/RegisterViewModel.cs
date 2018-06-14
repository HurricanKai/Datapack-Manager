using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class RegisterViewModel
    {
        [StringLength(16)]
        [DisplayName("Name")]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match!")]
        [DisplayName("Confirm Password")]
        public string RepeatedPassword { get; set; }

        [DisplayName("Remember Me")]
        public bool Persistent { get; set; }
    }
}
