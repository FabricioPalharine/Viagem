using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AvaliacaoAerea
	{
		public AvaliacaoAerea ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAerea_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAerea_IdentificadorViagemAerea",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagemAerea { get; set; }

			public int? Nota { get; set; }

			public string Comentario { get; set; }

			public Usuario ItemUsuario { get; set; }

			public ViagemAerea ItemViagemAerea { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public AvaliacaoAerea Clone()
		{
			 return (AvaliacaoAerea) this.MemberwiseClone();
		}
	}

}
