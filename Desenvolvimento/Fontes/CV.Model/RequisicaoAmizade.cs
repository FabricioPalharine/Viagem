using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class RequisicaoAmizade
	{
		public RequisicaoAmizade ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="RequisicaoAmizade_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public int? IdentificadorUsuarioRequisitado { get; set; }

			public string EMail { get; set; }
			[SelfValidation]
private void ValidarEMail(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (EMail == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.RequisicaoAmizade_EMail, this, "EMail", null, null);
      results.AddResult(result);
  }
  else if (EMail.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.RequisicaoAmizade_EMail_Tamanho, this, "EMail", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="RequisicaoAmizade_Status",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Status { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Usuario ItemUsuarioRequisitado { get; set; }
		 public RequisicaoAmizade Clone()
		{
			 return (RequisicaoAmizade) this.MemberwiseClone();
		}
	}

}
