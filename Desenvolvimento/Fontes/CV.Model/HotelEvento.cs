using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class HotelEvento
	{
		public HotelEvento ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelEvento_IdentificadorHotel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorHotel { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelEvento_DataEvento",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataEvento { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelEvento_Tipo",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Tipo { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelEvento_DataAtualizacao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataAtualizacao { get; set; }

			public Hotel ItemHotel { get; set; }
		 public HotelEvento Clone()
		{
			 return (HotelEvento) this.MemberwiseClone();
		}
	}

}
