using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Refeicao
	{
		public Refeicao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Refeicao_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Refeicao_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Refeicao_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Refeicao_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			[StringLengthValidator(50,MessageTemplateResourceName="Refeicao_CodigoPlace_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string CodigoPlace { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Refeicao_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Refeicao_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Refeicao_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="Refeicao_Tipo_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Tipo { get; set; }

			public Cidade ItemCidade { get; set; }

			public IList<FotoRefeicao> Fotos { get; set; }

			public IList<GastoRefeicao> Gastos { get; set; }

			public Viagem ItemViagem { get; set; }

			public int? IdentificadorAtracao { get; set; }

			public Atracao ItemAtracao { get; set; }

			public IList<RefeicaoPedido> Pedidos { get; set; }
		 public Refeicao Clone()
		{
			 return (Refeicao) this.MemberwiseClone();
		}
	}

}
