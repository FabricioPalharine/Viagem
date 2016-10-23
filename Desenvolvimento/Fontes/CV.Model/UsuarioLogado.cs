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
	}
}