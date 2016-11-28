using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class CalendarioPrevisto
	{
        public String strHoraDataInicio
        {
            get { return DataInicio.HasValue ? DataInicio.Value.ToString("HH:mm:ss") : null; }
        }

        public String strHoraDataFim
        {
            get { return DataFim.HasValue ? DataFim.Value.ToString("HH:mm:ss") : null; }
        }
    }

}
