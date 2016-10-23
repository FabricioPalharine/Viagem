using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ViagemAereaAeroporto
	{
		public ViagemAereaAeroporto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_IdentificadorViagemAerea",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagemAerea { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			public string Aeroporto { get; set; }
			[SelfValidation]
private void ValidarAeroporto(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Aeroporto == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ViagemAereaAeroporto_Aeroporto, this, "Aeroporto", null, null);
      results.AddResult(result);
  }
  else if (Aeroporto.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ViagemAereaAeroporto_Aeroporto_Tamanho, this, "Aeroporto", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_TipoPonto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? TipoPonto { get; set; }

			public DateTime? DataChegada { get; set; }

			public DateTime? DataPartida { get; set; }

			public Cidade ItemCidade { get; set; }

			public ViagemAerea ItemViagemAerea { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="ViagemAereaAeroporto_CodigoPlace_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string CodigoPlace { get; set; }
		 public ViagemAereaAeroporto Clone()
		{
			 return (ViagemAereaAeroporto) this.MemberwiseClone();
		}
	}

}
