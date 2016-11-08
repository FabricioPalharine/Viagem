using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class HotelAvaliacao
	{
		public HotelAvaliacao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelAvaliacao_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelAvaliacao_IdentificadorHotel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorHotel { get; set; }

			[NotNullValidator(MessageTemplateResourceName="HotelAvaliacao_Nota",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Nota { get; set; }

			public string Comentario { get; set; }

			public Hotel ItemHotel { get; set; }

			public Usuario ItemUsuario { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public HotelAvaliacao Clone()
		{
			 return (HotelAvaliacao) this.MemberwiseClone();
		}
	}

}
