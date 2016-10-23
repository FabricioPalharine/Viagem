using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Atracao
	{
		public Atracao ()
		{
		}

			public int? Identificador { get; set; }

			public int? IdentificadorAtracaoPai { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Atracao_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Atracao_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Atracao_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Atracao_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			[StringLengthValidator(50,MessageTemplateResourceName="Atracao_CodigoPlace_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string CodigoPlace { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Atracao_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Atracao_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			public DateTime? Chegada { get; set; }

			public DateTime? Partida { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="Atracao_Tipo_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Tipo { get; set; }

			public IList<Atracao> Atracoes { get; set; }

			public IList<AvaliacaoAtracao> Avaliacoes { get; set; }

			public IList<FotoAtracao> Fotos { get; set; }

			public Cidade ItemCidade { get; set; }

			public Viagem ItemViagem { get; set; }

			public Atracao ItemAtracaoPai { get; set; }
		 public Atracao Clone()
		{
			 return (Atracao) this.MemberwiseClone();
		}
	}

}
