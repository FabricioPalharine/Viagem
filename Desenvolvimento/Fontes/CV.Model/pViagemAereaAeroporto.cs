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
    }

}
