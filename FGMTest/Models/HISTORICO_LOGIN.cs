//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FGMTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HISTORICO_LOGIN
    {
        public int HIST_LOG_ID { get; set; }
        public System.DateTime HIST_LOG_FECHA { get; set; }
        public string USU_CORREO { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
    }
}
