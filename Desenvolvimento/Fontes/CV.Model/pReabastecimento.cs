using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class Reabastecimento
	{
        public TimeSpan? Hora
        {
            get { return Data.HasValue ? Data.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }
    }

}
