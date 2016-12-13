using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Cidade : ObservableObject
    {
        private bool _Selecionada;
        private bool _Visivel = true;
        public int? Identificador { get; set; }
        public string Nome { get; set; }
        public string Estado { get; set; }
        public string NomePais { get; set; }

        public bool Selecionada
        {
            get
            {
                return _Selecionada;
            }

            set
            {
                SetProperty(ref _Selecionada, value);
            }
        }

        public bool Visivel
        {
            get
            {
                return _Visivel;
            }

            set
            {
                SetProperty(ref _Visivel, value);
            }
        }
    }
}
