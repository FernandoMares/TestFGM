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
    using System.ComponentModel.DataAnnotations;

    public partial class PRODUCTO
    {
        public int PROD_ID { get; set; }
        
        public string PROD_NOMBRE { get; set; }
        
        public string PROD_DESCRIPCION { get; set; }
        
        public decimal PROD_PRECIO { get; set; }
        public Nullable<bool> PROD_ELIMINADO { get; set; }
    }
}
