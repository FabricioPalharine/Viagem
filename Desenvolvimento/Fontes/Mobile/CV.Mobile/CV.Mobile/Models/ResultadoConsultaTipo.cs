using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ResultadoConsultaTipo<T>
    {
        public ResultadoConsultaTipo()
        {
            Sucesso = true;
        }
        public int TotalRegistros { get; set; }
        public List<T> Lista { get; set; }
        public bool Sucesso { get; set; }
    }
}
