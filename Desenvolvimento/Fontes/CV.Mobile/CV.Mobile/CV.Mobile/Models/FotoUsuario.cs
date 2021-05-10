using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Models
{
    public class FotoUsuario
    {
        public int? Identificador { get; set; }
        public int? IdentificadorFoto { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string CodigoGoogle { get; set; }
        public Foto ItemFoto { get; set; }
    }
}
