using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Cidade
	{
		public Cidade ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Cidade_IdentificadorPais",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorPais { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Cidade_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Cidade_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			public string Estado { get; set; }
			[SelfValidation]
private void ValidarEstado(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Estado == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Cidade_Estado, this, "Estado", null, null);
      results.AddResult(result);
  }
  else if (Estado.Length > 100)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Cidade_Estado_Tamanho, this, "Estado", null, null);
      results.AddResult(result);
  }
}

			public Pais ItemPais { get; set; }
		 public Cidade Clone()
		{
			 return (Cidade) this.MemberwiseClone();
		}
	}

}
