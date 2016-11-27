using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AporteDinheiro
	{
		public AporteDinheiro ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AporteDinheiro_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AporteDinheiro_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AporteDinheiro_Valor",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Valor { get; set; }

			public int? Moeda { get; set; }

			public DateTime? DataAporte { get; set; }

			public decimal? Cotacao { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public int? IdentificadorGasto { get; set; }

			public Gasto ItemGasto { get; set; }
		 public AporteDinheiro Clone()
		{
			 return (AporteDinheiro) this.MemberwiseClone();
		}
	}

}
