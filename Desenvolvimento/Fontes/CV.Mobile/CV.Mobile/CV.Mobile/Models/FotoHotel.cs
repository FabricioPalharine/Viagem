
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class FotoHotel : ObservableObject
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorHotel { get; set; }
        public int? IdentificadorFoto { get; set; }

        public Hotel ItemHotel { get; set; }

        public Foto ItemFoto { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
    }
}
