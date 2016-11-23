using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CarroEvento
	{
		public CarroEvento ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_Inicio",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Inicio { get; set; }

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			public int? IdentificadorCidade { get; set; }

			public Cidade ItemCidade { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public DateTime? Data { get; set; }

			public int? Odometro { get; set; }
		 public CarroEvento Clone()
		{
			 return (CarroEvento) this.MemberwiseClone();
		}
	}

}
