using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class UsuarioGasto
	{
        public string NomeUsuario
        {
            get
            {
                return ItemUsuario?.Nome;
            }
        }
    }

}
