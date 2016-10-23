using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CotacaoMoeda
	{
		public CotacaoMoeda ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CotacaoMoeda_Moeda",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? Moeda { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CotacaoMoeda_DataCotacao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataCotacao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CotacaoMoeda_ValorCotacao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? ValorCotacao { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CotacaoMoeda_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public Viagem ItemViagem { get; set; }
		 public CotacaoMoeda Clone()
		{
			 return (CotacaoMoeda) this.MemberwiseClone();
		}
	}

}
