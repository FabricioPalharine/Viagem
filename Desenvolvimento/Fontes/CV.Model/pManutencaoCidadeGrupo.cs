using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class ManutencaoCidadeGrupo
    {
        public int? IdentificadorCidade { get; set; }
        public IEnumerable<int?> CidadesFilhas { get; set; }
        public bool Edicao { get; set; }
    }
}
