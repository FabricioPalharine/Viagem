using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Gasto
	{
		public Gasto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Gasto_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Gasto_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public string Descricao { get; set; }
			[SelfValidation]
private void ValidarDescricao(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Descricao == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Gasto_Descricao, this, "Descricao", null, null);
      results.AddResult(result);
  }
  else if (Descricao.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Gasto_Descricao_Tamanho, this, "Descricao", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Gasto_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Gasto_Valor",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Valor { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Gasto_Especie",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Especie { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Gasto_Moeda",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Moeda { get; set; }

			public DateTime? DataPagamento { get; set; }

			public bool? Dividido { get; set; }

			public bool? ApenasBaixa { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			public int? IdentificadorCidade { get; set; }

			public IList<GastoAtracao> Atracoes { get; set; }

			public IList<GastoHotel> Hoteis { get; set; }

			public IList<GastoCompra> Compras { get; set; }

			public IList<AluguelGasto> Alugueis { get; set; }

			public IList<GastoRefeicao> Refeicoes { get; set; }

			public IList<GastoViagemAerea> ViagenAereas { get; set; }

			public IList<GastoDividido> Usuarios { get; set; }

			public IList<ReabastecimentoGasto> Reabastecimentos { get; set; }

			public Cidade ItemCidade { get; set; }
		 public Gasto Clone()
		{
			 return (Gasto) this.MemberwiseClone();
		}
	}

}
