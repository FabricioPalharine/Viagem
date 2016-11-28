using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Pais
	{
		public Pais ()
		{
		}

			public int? Identificador { get; set; }

			public string Sigla { get; set; }
			[SelfValidation]
private void ValidarSigla(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Sigla == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Pais_Sigla, this, "Sigla", null, null);
      results.AddResult(result);
  }
  else if (Sigla.Length > 3)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Pais_Sigla_Tamanho, this, "Sigla", null, null);
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
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Pais_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 50)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Pais_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}
		 public Pais Clone()
		{
			 return (Pais) this.MemberwiseClone();
		}
	}

}
