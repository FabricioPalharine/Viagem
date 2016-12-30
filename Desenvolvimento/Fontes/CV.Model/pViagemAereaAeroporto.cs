using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class ViagemAereaAeroporto
	{
        public string strHoraChegada
        {
            get { return DataChegada.HasValue ? DataChegada.Value.ToString("HH:mm:ss") : null; }

        }

        public string strHoraPartida
        {
            get { return DataPartida.HasValue ? DataPartida.Value.ToString("HH:mm:ss") : null; }

        }

        public TimeSpan? HoraChegada
        {
            get { return DataChegada.HasValue ? DataChegada.Value.TimeOfDay : new Nullable<TimeSpan>(); }
        }
        public TimeSpan? HoraPartida
        {
            get { return DataPartida.HasValue ? DataPartida.Value.TimeOfDay : new Nullable<TimeSpan>(); }
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
