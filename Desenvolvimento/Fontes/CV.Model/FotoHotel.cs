using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class FotoHotel
	{
		public FotoHotel ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoHotel_IdentificadorFoto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorFoto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoHotel_IdentificadorHotel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorHotel { get; set; }

			public Foto ItemFoto { get; set; }

			public Hotel ItemHotel { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public FotoHotel Clone()
		{
			 return (FotoHotel) this.MemberwiseClone();
		}
	}

}
