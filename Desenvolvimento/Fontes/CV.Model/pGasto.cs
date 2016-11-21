using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;
using CV.Model.Dominio;

namespace CV.Model
{

	public partial class Gasto
	{
        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }

    }

}
