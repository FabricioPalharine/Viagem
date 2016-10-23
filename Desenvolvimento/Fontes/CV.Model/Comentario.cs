using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Comentario
	{
		public Comentario ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public int? IdentificadorCidade { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_Texto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Texto { get; set; }

			public Cidade ItemCidade { get; set; }

			public Viagem ItemViagem { get; set; }
		 public Comentario Clone()
		{
			 return (Comentario) this.MemberwiseClone();
		}
	}

}
