using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoHotel
	{
		public GastoHotel ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoHotel_IdentificadorHotel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorHotel { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoHotel_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			public Gasto ItemGasto { get; set; }

			public Hotel ItemHotel { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public GastoHotel Clone()
		{
			 return (GastoHotel) this.MemberwiseClone();
		}
	}

}
