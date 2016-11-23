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

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public DateTime? DataRetirada { get; set; }

			public DateTime? DataDevolucao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Carro_Descricao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Descricao { get; set; }

			public int? IdentificadorCarroEventoRetirada { get; set; }

			public int? IdentificadorCarroEventoDevolucao { get; set; }

			public CarroEvento ItemCarroEventoRetirada { get; set; }

			public CarroEvento ItemCarroEventoDevolucao { get; set; }

			public IList<CarroDeslocamento> Deslocamentos { get; set; }
		 public Carro Clone()
		{
			 return (Carro) this.MemberwiseClone();
		}
	}

}
