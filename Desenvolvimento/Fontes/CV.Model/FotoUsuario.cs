using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class FotoUsuario
    {
        public int? Identificador { get; set; }
        public int? IdentificadorFoto { get; set; }
        public int? IdentificadorUsuario { get;set; }
        public string CodigoGoogle { get; set; }
        public Foto ItemFoto { get; set; }
    }
}
