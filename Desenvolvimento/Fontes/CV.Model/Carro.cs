using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Carro
	{
		public Carro ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Carro_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="Carro_Locadora_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Locadora { get; set; }

			public string Modelo { get; set; }
			[SelfValidation]
private void ValidarModelo(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Modelo == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Carro_Modelo, this, "Modelo", null, null);
      results.AddResult(result);
  }
  else if (Modelo.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Carro_Modelo_Tamanho, this, "Modelo", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Carro_KM",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? KM { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Carro_Alugado",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Alugado { get; set; }

			public IList<AluguelGasto> Gastos { get; set; }

			public IList<Reabastecimento> Reabastecimentos { get; set; }

			public IList<AvaliacaoAluguel> Avaliacoes { get; set; }

			public IList<CarroEvento> Eventos { get; set; }

			public Viagem ItemViagem { get; set; }
		 public Carro Clone()
		{
			 return (Carro) this.MemberwiseClone();
		}
	}

}
