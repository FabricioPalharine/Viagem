using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Hotel
	{
		public Hotel ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Hotel_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Hotel_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			[StringLengthValidator(50,MessageTemplateResourceName="Hotel_CodigoPlace_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string CodigoPlace { get; set; }

			public DateTime? DataEntrada { get; set; }

			public DateTime? DataSaidia { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_EntradaPrevista",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? EntradaPrevista { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Hotel_SaidaPrevista",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? SaidaPrevista { get; set; }

			public Cidade ItemCidade { get; set; }

			public IList<FotoHotel> Fotos { get; set; }

			public IList<GastoHotel> Gastos { get; set; }

			public IList<HotelAvaliacao> Avaliacoes { get; set; }

			public Viagem ItemViagem { get; set; }
		 public Hotel Clone()
		{
			 return (Hotel) this.MemberwiseClone();
		}
	}

}
