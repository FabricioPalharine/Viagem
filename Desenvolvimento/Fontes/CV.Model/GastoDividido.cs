using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoDividido
	{
		public GastoDividido ()
		{
		}

			public int? Identificador { get; set; }

			public int? IdentificadorUsuario { get; set; }

			public int? IdentificadorGasto { get; set; }

			public Usuario ItemUsuario { get; set; }
		 public GastoDividido Clone()
		{
			 return (GastoDividido) this.MemberwiseClone();
		}
	}

}
