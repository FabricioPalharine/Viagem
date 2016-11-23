using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class AvaliacaoAluguel
	{
		public AvaliacaoAluguel ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAluguel_IdentificadorUsuario",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorUsuario { get; set; }

			[NotNullValidator(MessageTemplateResourceName="AvaliacaoAluguel_IdentificadorCarro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarro { get; set; }

			public int? Nota { get; set; }

			public string Comentario { get; set; }

			public Carro ItemCarro { get; set; }

			public Usuario ItemUsuario { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public AvaliacaoAluguel Clone()
		{
			 return (AvaliacaoAluguel) this.MemberwiseClone();
		}
	}

}
