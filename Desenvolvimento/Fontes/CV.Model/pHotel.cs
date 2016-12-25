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

        public TimeSpan? HoraEntrada
        {
            get { return DataEntrada.HasValue ? DataEntrada.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }
        public TimeSpan? HoraSaida
        {
            get { return DataSaidia.HasValue ? DataSaidia.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }
    }

}
