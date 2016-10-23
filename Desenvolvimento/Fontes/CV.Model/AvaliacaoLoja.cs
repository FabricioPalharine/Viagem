using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AvaliacaoLoja
	{
		public AvaliacaoLoja ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoLoja_IdentificadorLoja",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorLoja { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoLoja_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoLoja_Nota",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Nota { get; set; }

			public string Comentario { get; set; }

			public Loja ItemLoja { get; set; }

			public Usuario ItemUsuario { get; set; }
		 public AvaliacaoLoja Clone()
		{
			 return (AvaliacaoLoja) this.MemberwiseClone();
		}
	}

}
