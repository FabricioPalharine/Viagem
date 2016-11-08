using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ViagemAerea
	{
		public ViagemAerea ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ViagemAerea_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public string CompanhiaAerea { get; set; }
			[SelfValidation]
private void ValidarCompanhiaAerea(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (CompanhiaAerea == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ViagemAerea_CompanhiaAerea, this, "CompanhiaAerea", null, null);
      results.AddResult(result);
  }
  else if (CompanhiaAerea.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ViagemAerea_CompanhiaAerea_Tamanho, this, "CompanhiaAerea", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="ViagemAerea_DataPrevista",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataPrevista { get; set; }

			public DateTime? DataInicio { get; set; }

			public DateTime? DataFim { get; set; }

			public Viagem ItemViagem { get; set; }

			public IList<GastoViagemAerea> Gastos { get; set; }

			public IList<ViagemAereaAeroporto> Aeroportos { get; set; }

			public IList<AvaliacaoAerea> Avaliacoes { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public ViagemAerea Clone()
		{
			 return (ViagemAerea) this.MemberwiseClone();
		}
	}

}
