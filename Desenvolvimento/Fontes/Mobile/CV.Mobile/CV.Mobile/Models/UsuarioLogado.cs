using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class UsuarioLogado
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string CodigoGoogle { get; set; }
        public string Email { get; set; }
        public string LinkFoto { get; set; }
        public string AuthenticationToken { get; set; }
    }
}
