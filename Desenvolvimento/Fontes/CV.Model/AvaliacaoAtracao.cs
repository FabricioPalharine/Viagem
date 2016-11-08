using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AvaliacaoAtracao
	{
		public AvaliacaoAtracao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAtracao_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public int? Nota { get; set; }

			public string Comentario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAtracao_IdentificadorAtracao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorAtracao { get; set; }

			public Atracao ItemAtracao { get; set; }

			public Usuario ItemUsuario { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public AvaliacaoAtracao Clone()
		{
			 return (AvaliacaoAtracao) this.MemberwiseClone();
		}
	}

}
