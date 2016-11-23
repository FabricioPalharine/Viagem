using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CarroDeslocamentoUsuario
	{
		public CarroDeslocamentoUsuario ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroDeslocamentoUsuario_IdentificadorCarroDeslocamento",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarroDeslocamento { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroDeslocamentoUsuario_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public CarroDeslocamento ItemCarroDeslocamento { get; set; }

			public Usuario ItemUsuario { get; set; }
		 public CarroDeslocamentoUsuario Clone()
		{
			 return (CarroDeslocamentoUsuario) this.MemberwiseClone();
		}
	}

}
