using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Amigo
	{
		public Amigo ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Amigo_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public int? IdentificadorAmigo { get; set; }

			public string EMail { get; set; }
			[SelfValidation]
private void ValidarEMail(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (EMail == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Amigo_EMail, this, "EMail", null, null);
      results.AddResult(result);
  }
  else if (EMail.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Amigo_EMail_Tamanho, this, "EMail", null, null);
      results.AddResult(result);
  }
}

			public Usuario ItemUsuario { get; set; }

			public Usuario ItemAmigo { get; set; }
		 public Amigo Clone()
		{
			 return (Amigo) this.MemberwiseClone();
		}
	}

}
