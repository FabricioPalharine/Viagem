using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class GastoAtracao: ObservableObject
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }

        public int? IdentificadorGasto { get; set; }

        public int? IdentificadorAtracao { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public Gasto ItemGasto { get; set; }

    }
}
