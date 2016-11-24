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

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_Texto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Texto { get; set; }

			public Cidade ItemCidade { get; set; }

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public Usuario ItemUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Comentario_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }
		 public Comentario Clone()
		{
			 return (Comentario) this.MemberwiseClone();
		}
	}

}
