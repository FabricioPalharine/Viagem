using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class Atracao
	{
        public String strHoraChegada
        {
            get { return Chegada.HasValue ? Chegada.Value.ToString("HH:mm:ss") : null; }
        }

        public String strHoraPartida
        {
            get { return Partida.HasValue ? Partida.Value.ToString("HH:mm:ss") : null; }
        }
    }

}
