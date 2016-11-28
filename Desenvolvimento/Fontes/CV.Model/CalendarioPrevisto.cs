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

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_DataInicio",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataInicio { get; set; }

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

			public string CodigoPlace { get; set; }

			public Viagem ItemViagem { get; set; }

			public string Tipo { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_Prioridade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Prioridade { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_DataFim",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataFim { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CalendarioPrevisto_AvisarHorario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? AvisarHorario { get; set; }
		 public CalendarioPrevisto Clone()
		{
			 return (CalendarioPrevisto) this.MemberwiseClone();
		}
	}

}
