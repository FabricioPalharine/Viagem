using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Viagem
	{
		public Viagem ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			public string Nome { get; set; }
			[SelfValidation]
private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
{
 if (Nome == null)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Viagem_Nome, this, "Nome", null, null);
      results.AddResult(result);
  }
  else if (Nome.Length > 200)
  {
      Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
            new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Viagem_Nome_Tamanho, this, "Nome", null, null);
      results.AddResult(result);
  }
}

			[NotNullValidator(MessageTemplateResourceName="Viagem_DataInicio",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataInicio { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_DataFim",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataFim { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_Aberto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Aberto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_UnidadeMetrica",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? UnidadeMetrica { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_QuantidadeParticipantes",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? QuantidadeParticipantes { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_PublicaGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? PublicaGasto { get; set; }

			public decimal? PercentualIOF { get; set; }

			public Usuario ItemUsuario { get; set; }

			public IList<ParticipanteViagem> Participantes { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_Moeda",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Moeda { get; set; }

			public IList<UsuarioGasto> UsuariosGastos { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Viagem_DataAlteracao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataAlteracao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public string CodigoAlbum { get; set; }
		 public Viagem Clone()
		{
			 return (Viagem) this.MemberwiseClone();
		}
	}

}
