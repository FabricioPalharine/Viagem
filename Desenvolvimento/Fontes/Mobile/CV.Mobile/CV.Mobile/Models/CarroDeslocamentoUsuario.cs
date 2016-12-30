using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CarroDeslocamentoUsuario : ObservableObject
    {
        public int? Id { get; set; }
        public string NomeUsuario { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorCarroDeslocamento { get; set; }

        public int? IdentificadorUsuario { get; set; }

        public CarroDeslocamento ItemCarroDeslocamento { get; set; }

        public Usuario ItemUsuario { get; set; }
        public CarroDeslocamentoUsuario Clone()
        {
            return (CarroDeslocamentoUsuario)this.MemberwiseClone();
        }
    }
}
