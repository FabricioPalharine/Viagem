using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoRefeicao
	{
		public GastoRefeicao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoRefeicao_IdentificadorRefeicao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorRefeicao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoRefeicao_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			public Gasto ItemGasto { get; set; }

			public Refeicao ItemRefeicao { get; set; }
		 public GastoRefeicao Clone()
		{
			 return (GastoRefeicao) this.MemberwiseClone();
		}
	}

}
