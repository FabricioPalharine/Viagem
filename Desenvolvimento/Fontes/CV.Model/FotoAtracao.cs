using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class FotoAtracao
	{
		public FotoAtracao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoAtracao_IdentificadorAtracao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorAtracao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoAtracao_IdentificadorFoto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorFoto { get; set; }

			public Atracao ItemAtracao { get; set; }

			public Foto ItemFoto { get; set; }
		 public FotoAtracao Clone()
		{
			 return (FotoAtracao) this.MemberwiseClone();
		}
	}

}
