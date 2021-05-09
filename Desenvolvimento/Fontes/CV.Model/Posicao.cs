using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Posicao
	{
		public Posicao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_DataGMT",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataGMT { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Posicao_DataLocal",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataLocal { get; set; }
		 public Posicao Clone()
		{
			 return (Posicao) this.MemberwiseClone();
		}
	}

}
