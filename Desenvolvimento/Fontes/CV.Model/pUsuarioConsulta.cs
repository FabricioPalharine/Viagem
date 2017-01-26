using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class UsuarioConsulta
    {
        public string Nome { get; set; }
        public int? Identificador { get; set; }
        public int? Nota { get; set; }
        public string Comentario { get; set; }
        public string Pedido { get; set; }
    }
}
