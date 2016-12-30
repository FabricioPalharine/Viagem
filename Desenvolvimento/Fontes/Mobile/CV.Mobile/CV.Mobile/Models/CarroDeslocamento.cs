using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CarroDeslocamento: ObservableObject
    {
        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorCarro { get; set; }

        public int? IdentificadorCarroEventoPartida { get; set; }

        public int? IdentificadorCarroEventoChegada { get; set; }

        public CarroEvento ItemCarroEventoPartida { get; set; }

        public CarroEvento ItemCarroEventoChegada { get; set; }

        public ObservableRangeCollection<CarroDeslocamentoUsuario> Usuarios { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public string Observacao { get; set; }

        public Carro ItemCarro { get; set; }

        public CarroDeslocamento Clone()
        {
            return (CarroDeslocamento)this.MemberwiseClone();
        }
    }
}
