using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ManutencaoCidadeGrupo
    {
        public int? IdentificadorCidade { get; set; }
        public ObservableCollection<int?> CidadesFilhas { get; set; }
        public bool Edicao { get; set; }
    }
}
