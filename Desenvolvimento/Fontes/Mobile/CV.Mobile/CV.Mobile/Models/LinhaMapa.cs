using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class LinhaMapa
    {
        public ICollection<PontoMapa> Pontos { get; set; }
        public string Tipo { get; set; }
        public DateTime? Data { get; set; }
    }
}
