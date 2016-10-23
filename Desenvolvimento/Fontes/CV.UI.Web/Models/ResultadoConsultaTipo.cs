using CV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.UI.Web.Models
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
        public MensagemErro[] Mensagens { get; set; }
    }
}