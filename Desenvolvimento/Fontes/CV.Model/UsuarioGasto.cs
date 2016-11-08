using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class UsuarioGasto
	{
		public UsuarioGasto ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="UsuarioGasto_IdentificadorViagem",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorViagem { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public Viagem ItemViagem { get; set; }

			public Usuario ItemUsuario { get; set; }
		 public UsuarioGasto Clone()
		{
			 return (UsuarioGasto) this.MemberwiseClone();
		}
	}

}
