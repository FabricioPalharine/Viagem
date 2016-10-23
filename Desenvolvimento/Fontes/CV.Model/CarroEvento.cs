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

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_Tipo",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Tipo { get; set; }

			public int? Odometro { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_IdentificadorCarro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarro { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroEvento_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			public Carro ItemCarro { get; set; }

			public Cidade ItemCidade { get; set; }
		 public CarroEvento Clone()
		{
			 return (CarroEvento) this.MemberwiseClone();
		}
	}

}
