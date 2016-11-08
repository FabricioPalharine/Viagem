using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class ParticipanteViagem
	{
		public ParticipanteViagem ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="ParticipanteViagem_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public Usuario ItemUsuario { get; set; }

			public Viagem ItemViagem { get; set; }
		 public ParticipanteViagem Clone()
		{
			 return (ParticipanteViagem) this.MemberwiseClone();
		}
	}

}
