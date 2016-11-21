using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoAtracao
	{
		public GastoAtracao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoAtracao_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoAtracao_IdentificadorAtracao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorAtracao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoAtracao_DataAtualizacao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public Gasto ItemGasto { get; set; }

			public Atracao ItemAtracao { get; set; }
		 public GastoAtracao Clone()
		{
			 return (GastoAtracao) this.MemberwiseClone();
		}
	}

}
