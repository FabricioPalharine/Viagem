using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ReabastecimentoGasto
	{
		public ReabastecimentoGasto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ReabastecimentoGasto_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ReabastecimentoGasto_IdentificadorReabastecimento",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorReabastecimento { get; set; }

			public Reabastecimento ItemReabastecimento { get; set; }

			public Gasto ItemGasto { get; set; }
		 public ReabastecimentoGasto Clone()
		{
			 return (ReabastecimentoGasto) this.MemberwiseClone();
		}
	}

}
