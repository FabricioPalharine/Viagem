using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class ConsultaSolicitacao
    {
        public int? IdentificadorUsuario { get; set; }
        public string EMail { get; set; }
        public string Nome { get; set; }
        public int IdentificadorRequisicao { get; set; }
       
    }
}
