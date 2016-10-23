using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ParticipanteViagem
	{
		public ParticipanteViagem ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ParticipanteViagem_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public string EMail { get; set; }
			[SelfValidation]
private void ValidarEMail(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (EMail == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ParticipanteViagem_EMail, this, "EMail", null, null);
      results.AddResult(result);
  }
  else if (EMail.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.ParticipanteViagem_EMail_Tamanho, this, "EMail", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="ParticipanteViagem_Status",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Status { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }
		 public ParticipanteViagem Clone()
		{
			 return (ParticipanteViagem) this.MemberwiseClone();
		}
	}

}
