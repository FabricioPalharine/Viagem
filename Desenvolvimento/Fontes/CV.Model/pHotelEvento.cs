using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class HotelEvento
	{
        public String strHoraEntrada
        {
            get { return DataEntrada.HasValue ? DataEntrada.Value.ToString("HH:mm:ss") : null; }
        }
        public String strHoraSaida
        {
            get { return DataSaida.HasValue ? DataSaida.Value.ToString("HH:mm:ss") : null; }
        }

        public TimeSpan? HoraEntrada
        {
            get { return DataEntrada.HasValue ? DataEntrada.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }
        public TimeSpan? HoraSaida
        {
            get { return DataSaida.HasValue ? DataSaida.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }

        public string NomeUsuario
        {
            get
            {
                return (ItemUsuario ?? new Usuario()).Nome;
            }
        }
    }

}
