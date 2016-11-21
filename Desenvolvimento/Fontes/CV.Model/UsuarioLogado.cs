using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CV.Model
{
	public class UsuarioLogado
	{

		public int Codigo { get; set; }

		public string Nome { get; set; }

		public string Login { get; set; }

		public string Email { get; set; }

		public bool Ativo { get; set; }

		public int LogAcesso { get; set; }

		public string WorkstationId { get; set; }

		public DateTime UltimoAcesso { get; set; }

		public DateTime DataHoraAcesso { get; set; }

    public bool Sucesso { get; set; }
    
    public MensagemErro[] Mensagens { get; set; }
    
    public string AuthenticationToken {get;set;}

        public string Cultura { get; set; }

        public List<Viagem> Viagens { get; set; }

        public int? IdentificadorViagem { get; set; }
        public string NomeViagem { get; set; }
        public bool PermiteEdicao { get; set; }
        public bool VerCustos { get; set; }

        public bool Aberto { get; set; }
        public string LinkFoto { get; set; }
        public string CodigoGoogle { get; set; }

    }
}