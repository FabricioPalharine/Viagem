using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ConsultaAmigo
    {
        public int? IdentificadorUsuario { get; set; }
        public string EMail { get; set; }
        public string Nome { get; set; }
        public bool Seguidor { get; set; }
        public bool Seguido { get; set; }

        public int Acao { get; set; }
    }
}
