using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class RequisicaoAmizade
    {
        public int? Identificador { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public int? IdentificadorUsuarioRequisitado { get; set; }
        public string EMail { get; set; }
        public int? Status { get; set; }
        public Usuario ItemUsuario { get; set; }
        public Usuario ItemUsuarioRequisitado { get; set; }
        public string EMailUsuario { get; set; }
        public string NomeUsuario { get; set; }
    }
}
