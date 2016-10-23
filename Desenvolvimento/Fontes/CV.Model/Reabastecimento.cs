using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class Reabastecimento
	{
		public Reabastecimento ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_IdentificadorCarro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarro { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_IdentificadorCidade",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCidade { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_Litro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Litro { get; set; }

			public decimal? QuantidadeReabastecida { get; set; }

			public IList<ReabastecimentoGasto> Gastos { get; set; }

			public Carro ItemCarro { get; set; }

			public Cidade Cidade { get; set; }
		 public Reabastecimento Clone()
		{
			 return (Reabastecimento) this.MemberwiseClone();
		}
	}

}
