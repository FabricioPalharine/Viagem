using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ViagemAerea: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorViagem { get; set; }

        public string CompanhiaAerea { get; set; }
      

        public DateTime? DataPrevista { get; set; }
        [Ignore]
        public ObservableRangeCollection<GastoViagemAerea> Gastos { get; set; }
        [Ignore]
        public ObservableRangeCollection<ViagemAereaAeroporto> Aeroportos { get; set; }
        [Ignore]
        public ObservableRangeCollection<AvaliacaoAerea> Avaliacoes { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public int? Tipo { get; set; }

        public string Descricao { get; set; }
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
