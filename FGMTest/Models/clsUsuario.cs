using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FGMTest.Models;

namespace FGMTest.Models
{
    public class clsUsuario
    {
        [Required]
        public string nombre { get; set; }
        [Required]
        public string correo { get; set; }
        [Required]
        public string password { get; set; }
        public bool activo { get; set; }
    }
}