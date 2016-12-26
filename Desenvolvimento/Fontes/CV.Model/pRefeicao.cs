using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class Refeicao
	{
        public String strHora
        {
            get { return Data.HasValue ? Data.Value.ToString("HH:mm:ss") : null; }
        }

        public TimeSpan? Hora
        {
            get { return Data.HasValue ? Data.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }

        public string NomeCidade
        {
            get
            {
                return (ItemCidade ?? new Cidade()).Nome;
            }
        }
    }

}
