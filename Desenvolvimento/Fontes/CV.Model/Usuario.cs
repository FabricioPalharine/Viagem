using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Usuario
	{
		public Usuario ()
		{
		}

			public int? Identificador { get; set; }

			public string EMail { get; set; }
			[SelfValidation]
private void ValidarEMail(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (EMail == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_EMail, this, "EMail", null, null);
      results.AddResult(result);
  }
  else if (EMail.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_EMail_Tamanho, this, "EMail", null, null);
      results.AddResult(result);
  }
}

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			public string Token { get; set; }
			[SelfValidation]
private void ValidarToken(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Token == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Token, this, "Token", null, null);
      results.AddResult(result);
  }
  else if (Token.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Token_Tamanho, this, "Token", null, null);
      results.AddResult(result);
  }
}

			public string RefreshToken { get; set; }
			[SelfValidation]
private void ValidarRefreshToken(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (RefreshToken == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_RefreshToken, this, "RefreshToken", null, null);
      results.AddResult(result);
  }
  else if (RefreshToken.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_RefreshToken_Tamanho, this, "RefreshToken", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Usuario_DataToken",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataToken { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Usuario_Lifetime",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Lifetime { get; set; }

			public string Codigo { get; set; }
			[SelfValidation]
private void ValidarCodigo(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Codigo == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Codigo, this, "Codigo", null, null);
      results.AddResult(result);
  }
  else if (Codigo.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Usuario_Codigo_Tamanho, this, "Codigo", null, null);
      results.AddResult(result);
  }
}
		 public Usuario Clone()
		{
			 return (Usuario) this.MemberwiseClone();
		}
	}

}
