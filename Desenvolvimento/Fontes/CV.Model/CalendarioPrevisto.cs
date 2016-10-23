using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CalendarioPrevisto
	{
		public CalendarioPrevisto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }

			[StringLengthValidator(5,MessageTemplateResourceName="CalendarioPrevisto_Inicio_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Inicio { get; set; }

			[StringLengthValidator(5,MessageTemplateResourceName="CalendarioPrevisto_Fim_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Fim { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.CalendarioPrevisto_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.CalendarioPrevisto_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="CalendarioPrevisto_CodigoPlace_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string CodigoPlace { get; set; }

			public Viagem ItemViagem { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="CalendarioPrevisto_Tipo_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Tipo { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_Prioridade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Prioridade { get; set; }
		 public CalendarioPrevisto Clone()
		{
			 return (CalendarioPrevisto) this.MemberwiseClone();
		}
	}

}
