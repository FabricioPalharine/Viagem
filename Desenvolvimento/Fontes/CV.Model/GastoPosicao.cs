using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoPosicao
	{
		public GastoPosicao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoPosicao_Latitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Latitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoPosicao_Longitude",MessageTemplateResourceType=typeof(MensagemModelo))]
			public decimal? Longitude { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoPosicao_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			public int? IdentificadorCidade { get; set; }

			public Cidade ItemCidade { get; set; }

			public Gasto ItemGasto { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public GastoPosicao Clone()
		{
			 return (GastoPosicao) this.MemberwiseClone();
		}
	}

}
