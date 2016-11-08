using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CidadeGrupo
	{
		public CidadeGrupo ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CidadeGrupo_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CidadeGrupo_IdentificadorCidadeFilha",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidadeFilha { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CidadeGrupo_IdentificadorCidadePai",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidadePai { get; set; }

			public Cidade ItemCidadeFilha { get; set; }

			public Cidade ItemCidadePai { get; set; }

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public CidadeGrupo Clone()
		{
			 return (CidadeGrupo) this.MemberwiseClone();
		}
	}

}
