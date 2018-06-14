using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class LoginViewModel
    {
        [StringLength(16)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [PasswordPropertyText]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool Persistent { get; set; }
    }
}
