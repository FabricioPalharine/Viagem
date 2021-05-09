using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Loja
	{
		public Loja ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Loja_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Loja_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Loja_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			public string CodigoPlace { get; set; }


			public Viagem ItemViagem { get; set; }

			public int? IdentificadorAtracao { get; set; }

			public Atracao ItemAtracao { get; set; }

			public IList<AvaliacaoLoja> Avaliacoes { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public int? IdentificadorCidade { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "Loja_Data", MessageTemplateResourceType = typeof(MensagemModelo))]
        public DateTime? Data { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "Loja_Valor", MessageTemplateResourceType = typeof(MensagemModelo))]
        public decimal? Valor { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "Loja_Moeda", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? Moeda { get; set; }

        public Cidade ItemCidade { get; set; }
		 public Loja Clone()
		{
			 return (Loja) this.MemberwiseClone();
		}
	}

}
