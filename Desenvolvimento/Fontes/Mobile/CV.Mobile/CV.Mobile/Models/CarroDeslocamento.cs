using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CarroDeslocamento: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorCarro { get; set; }

        public int? IdentificadorCarroEventoPartida { get; set; }

        public int? IdentificadorCarroEventoChegada { get; set; }
        [Ignore]
        public CarroEvento ItemCarroEventoPartida { get; set; }
        [Ignore]
        public CarroEvento ItemCarroEventoChegada { get; set; }
        [Ignore]
        public ObservableRangeCollection<CarroDeslocamentoUsuario> Usuarios { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public string Observacao { get; set; }
        private bool _Atualizado = true;

        public bool AtualizadoBanco
        {
            get
            {
                return _Atualizado;
            }

            set
            {
                _Atualizado = value;
            }
        }
        public CarroDeslocamento Clone()
        {
            return (CarroDeslocamento)this.MemberwiseClone();
        }
    }
}
