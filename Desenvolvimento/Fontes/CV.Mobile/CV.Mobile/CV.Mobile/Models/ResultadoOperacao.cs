using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Models
{
    public partial class ResultadoOperacao
    {
        public bool Sucesso { get; set; }
        public MensagemErro[] Mensagens { get; set; } = new MensagemErro[] { new MensagemErro() { Mensagem = "Erro Inexperado" } };
        public int? IdentificadorRegistro { get; set; }

        public object ItemRegistro { get; set; }
    }
}
