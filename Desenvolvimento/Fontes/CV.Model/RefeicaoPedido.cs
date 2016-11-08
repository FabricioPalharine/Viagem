using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class RefeicaoPedido
	{
		public RefeicaoPedido ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="RefeicaoPedido_IdentificadorRefeicao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorRefeicao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="RefeicaoPedido_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public string Pedido { get; set; }
			[SelfValidation]
private void ValidarPedido(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Pedido == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.RefeicaoPedido_Pedido, this, "Pedido", null, null);
      results.AddResult(result);
  }
  else if (Pedido.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.RefeicaoPedido_Pedido_Tamanho, this, "Pedido", null, null);
      results.AddResult(result);
  }
}

			public int? Nota { get; set; }

			public string Comentario { get; set; }

			public Refeicao ItemRefeicao { get; set; }

			public Usuario ItemUsuario { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public RefeicaoPedido Clone()
		{
			 return (RefeicaoPedido) this.MemberwiseClone();
		}
	}

}
