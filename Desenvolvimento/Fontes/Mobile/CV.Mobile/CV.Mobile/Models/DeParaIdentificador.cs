using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class DeParaIdentificador
    {
        public int? IdentificadorOrigem { get; set; }
        public int? IdentificadorDetino { get; set; }
        public string TipoObjeto { get; set; }
        public object ItemRetorno { get; set; }
    }
}
