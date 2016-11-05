using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Sugestao
	{
		public Sugestao ()
		{
		}

			public int? Identificador { get; set; }

			public string Local { get; set; }
			[SelfValidation]
private void ValidarLocal(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Local == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Sugestao_Local, this, "Local", null, null);
      results.AddResult(result);
  }
  else if (Local.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Sugestao_Local_Tamanho, this, "Local", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Sugestao_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Sugestao_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			public string Comentario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Sugestao_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Sugestao_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Sugestao_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Sugestao_Status",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Status { get; set; }

			public Cidade ItemCidade { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="Sugestao_Tipo_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Tipo { get; set; }
		 public Sugestao Clone()
		{
			 return (Sugestao) this.MemberwiseClone();
		}
	}

}
