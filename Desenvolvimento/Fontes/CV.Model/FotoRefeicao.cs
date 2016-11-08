using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class FotoRefeicao
	{
		public FotoRefeicao ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoRefeicao_IdentificadorFoto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorFoto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoRefeicao_IdentificadorRefeicao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorRefeicao { get; set; }

			public Foto ItemFoto { get; set; }

			public Refeicao ItemRefeicao { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public FotoRefeicao Clone()
		{
			 return (FotoRefeicao) this.MemberwiseClone();
		}
	}

}
