using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoViagemAerea
	{
		public GastoViagemAerea ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoViagemAerea_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoViagemAerea_IdentificadorViagemAerea",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagemAerea { get; set; }

			public Gasto ItemGasto { get; set; }

			public ViagemAerea ItemViagemAerea { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public GastoViagemAerea Clone()
		{
			 return (GastoViagemAerea) this.MemberwiseClone();
		}
	}

}
