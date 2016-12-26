using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class FotoRefeicao : ObservableObject
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorRefeicao { get; set; }
        public int? IdentificadorFoto { get; set; }

        public Refeicao ItemRefeicao { get; set; }

        public Foto ItemFoto { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
    }
}
