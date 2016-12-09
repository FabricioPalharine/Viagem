using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class RequisicaoAmizade
	{
        public string NomeUsuario
        {
            get
            {
                return this.ItemUsuario?.Nome;
            }
        }
        public string EMailUsuario
        {
            get
            {
                return this.ItemUsuario?.EMail;
            }
        }
	}

}
