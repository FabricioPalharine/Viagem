using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class PocoLogin
    {
        public string Login { get; set; }
        public string Senha { get; set; }
        public string CodigoEmpresa { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmarSenha { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string EMail { get; set; }
        public string CodigoValidacao { get; set; }
        public int? IdentificadorViagem { get; set; }
    }
}