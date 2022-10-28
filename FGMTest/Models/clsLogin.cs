using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FGMTest.Models
{
    public class clsLogin
    {
        [Required]
        public string correo{get;set;}
        [Required]
        public string password { get; set; }
    }
}