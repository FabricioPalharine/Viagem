using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public partial class AuthenticationToken
    {
        public int IdentificadorUsuario { get; set; }
        public string Token { get; set; }
        public string Cultura { get; set; }
        public int? IdentificadorViagem { get; set; }
        
    }
}