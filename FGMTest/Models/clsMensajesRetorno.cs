using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FGMTest.Models
{
    public class clsMensajesRetorno
    {
        public bool registroOK { get; set; }
        public string detalle { get; set; }
        public DateTime fechaIncidencia { get; set; }
    }
}