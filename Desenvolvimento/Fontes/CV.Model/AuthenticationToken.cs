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
        public int IdentificadorLogAcesso { get; set; }
        public string Cultura { get; set; }
        public int IdentificadorEmpresa { get; set; }
        public bool EmpresaPrincipal { get; set; }
    }
}