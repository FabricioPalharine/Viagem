using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Foto
	{
		public Foto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Foto_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public int? IdentificadorCidade { get; set; }

			public string Comentario { get; set; }

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Foto_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }

			public string LinkThumbnail { get; set; }


			public string LinkFoto { get; set; }
			[SelfValidation]
private void ValidarLinkFoto(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (LinkFoto == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Foto_LinkFoto, this, "LinkFoto", null, null);
      results.AddResult(result);
  }
  
}

			public string CodigoFoto { get; set; }
			[SelfValidation]
private void ValidarCodigoFoto(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (CodigoFoto == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Foto_CodigoFoto, this, "CodigoFoto", null, null);
      results.AddResult(result);
  }
  else if (CodigoFoto.Length > 500)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Foto_CodigoFoto_Tamanho, this, "CodigoFoto", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Foto_Video",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Video { get; set; }

			public string TipoArquivo { get; set; }
			[SelfValidation]
private void ValidarTipoArquivo(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (TipoArquivo == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Foto_TipoArquivo, this, "TipoArquivo", null, null);
      results.AddResult(result);
  }
  else if (TipoArquivo.Length > 20)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Foto_TipoArquivo_Tamanho, this, "TipoArquivo", null, null);
      results.AddResult(result);
  }
}

			public Cidade ItemCidade { get; set; }

			public IList<FotoHotel> Hoteis { get; set; }

			public IList<FotoItemCompra> ItensCompra { get; set; }

			public IList<FotoRefeicao> Refeicoes { get; set; }

			public IList<FotoAtracao> Atracoes { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public Foto Clone()
		{
			 return (Foto) this.MemberwiseClone();
		}
	}

}
