using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ListaCompra
	{
		public ListaCompra ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ListaCompra_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ListaCompra_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public int? IdentificadorUsuarioPedido { get; set; }

			public string Descricao { get; set; }
			[SelfValidation]
private void ValidarDescricao(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Descricao == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ListaCompra_Descricao, this, "Descricao", null, null);
      results.AddResult(result);
  }
  else if (Descricao.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ListaCompra_Descricao_Tamanho, this, "Descricao", null, null);
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
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ListaCompra_Marca, this, "Marca", null, null);
      results.AddResult(result);
  }
  else if (Marca.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ListaCompra_Marca_Tamanho, this, "Marca", null, null);
      results.AddResult(result);
  }
}

			public decimal? ValorMaximo { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ListaCompra_Moeda",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Moeda { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ListaCompra_Reembolsavel",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Reembolsavel { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ListaCompra_Comprado",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Comprado { get; set; }

			public IList<ItemCompra> ItensCompra { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Usuario ItemUsuarioPedido { get; set; }

			public Viagem Viagem { get; set; }

			[StringLengthValidator(200,MessageTemplateResourceName="ListaCompra_Destinatario_Tamanho",MessageTemplateResourceType=typeof(MensagemModelo))]
			public string Destinatario { get; set; }
		 public ListaCompra Clone()
		{
			 return (ListaCompra) this.MemberwiseClone();
		}
	}

}
