using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ItemCompra
	{
		public ItemCompra ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ItemCompra_IdentificadorGastoCompra",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGastoCompra { get; set; }

			public int? IdentificadorListaCompra { get; set; }

			public string Descricao { get; set; }
			[SelfValidation]
private void ValidarDescricao(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Descricao == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ItemCompra_Descricao, this, "Descricao", null, null);
      results.AddResult(result);
  }
  else if (Descricao.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ItemCompra_Descricao_Tamanho, this, "Descricao", null, null);
      results.AddResult(result);
  }
}

			public string Marca { get; set; }
			[SelfValidation]
private void ValidarMarca(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Marca == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ItemCompra_Marca, this, "Marca", null, null);
      results.AddResult(result);
  }
  else if (Marca.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ItemCompra_Marca_Tamanho, this, "Marca", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="ItemCompra_Valor",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Valor { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ItemCompra_Reembolsavel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Reembolsavel { get; set; }

			[StringLengthValidator(50,MessageTemplateResourceName="ItemCompra_Destinatario_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Destinatario { get; set; }

			public GastoCompra ItemGastoCompra { get; set; }

			public IList<FotoItemCompra> Fotos { get; set; }

			public ListaCompra ItemListaCompra { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public Usuario ItemUsuario { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public ItemCompra Clone()
		{
			 return (ItemCompra) this.MemberwiseClone();
		}
	}

}
