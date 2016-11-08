using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AluguelGasto
	{
		public AluguelGasto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AluguelGasto_IdentificadorCarro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarro { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AluguelGasto_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			public Carro ItemCarro { get; set; }

			public Gasto ItemGasto { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public AluguelGasto Clone()
		{
			 return (AluguelGasto) this.MemberwiseClone();
		}
	}

}
