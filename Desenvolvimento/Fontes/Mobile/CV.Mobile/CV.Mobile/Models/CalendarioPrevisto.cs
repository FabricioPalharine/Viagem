using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CalendarioPrevisto: ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorViagem { get; set; }

        public DateTime? DataInicio { get; set; }

        public string Nome { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string CodigoPlace { get; set; }

        public string Tipo { get; set; }

        public int? Prioridade { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

         public DateTime? DataFim { get; set; }

        public bool? AvisarHorario { get; set; }

        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFim { get; set; }

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

    }
}
