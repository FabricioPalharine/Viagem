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

			public int? IdentificadorViagemAerea { get; set; }

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

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAereaAeroporto_TipoPonto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? TipoPonto { get; set; }

			public DateTime? DataChegada { get; set; }

			public DateTime? DataPartida { get; set; }

			public Cidade ItemCidade { get; set; }

			public ViagemAerea ItemViagemAerea { get; set; }

			public string CodigoPlace { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public ViagemAereaAeroporto Clone()
		{
			 return (ViagemAereaAeroporto) this.MemberwiseClone();
		}
	}

}
