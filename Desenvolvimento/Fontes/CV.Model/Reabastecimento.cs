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

			public int? IdentificadorCidade { get; set; }

			public decimal? Latitude { get; set; }

			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_Litro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public bool? Litro { get; set; }

			public decimal? QuantidadeReabastecida { get; set; }

			public IList<ReabastecimentoGasto> Gastos { get; set; }

			public Carro ItemCarro { get; set; }

			public Cidade ItemCidade { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="Reabastecimento_Data",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? Data { get; set; }
		 public Reabastecimento Clone()
		{
			 return (Reabastecimento) this.MemberwiseClone();
		}
	}

}
