using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class Hotel
	{
        public String strHoraEntrada
        {
            get { return DataEntrada.HasValue ? DataEntrada.Value.ToString("HH:mm:ss") : null; }
        }
        public String strHoraSaida
        {
            get { return DataSaidia.HasValue ? DataSaidia.Value.ToString("HH:mm:ss") : null; }
        }
    }

}
