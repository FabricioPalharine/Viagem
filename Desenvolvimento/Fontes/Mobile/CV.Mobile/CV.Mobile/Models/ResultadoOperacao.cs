using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public partial class ResultadoOperacao
    {
        public bool Sucesso { get; set; }
        public MensagemErro[] Mensagens { get; set; }
        public int? IdentificadorRegistro { get; set; }

        public object ItemRegistro { get; set; }
    }
}
