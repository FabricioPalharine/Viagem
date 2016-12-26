using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Usuario: ObservableObject
    {
        public int? Identificador { get; set; }
        public string EMail { get; set; }
        public string Nome { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? DataToken { get; set; }
        public int? Lifetime { get; set; }
        public string Codigo { get; set; }

        public bool Selecionado
        {
            get
            {
                return _Selecionado;
            }

            set
            {
                SetProperty(ref _Selecionado, value);
            }
        }

        public string Complemento
        {
            get
            {
                return _Complemento;
            }

            set
            {
                SetProperty(ref _Complemento, value);
            }
        }

        private bool _Selecionado;
        private string _Complemento;
        
    }
}
