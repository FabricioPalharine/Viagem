using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Carro: ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public string Locadora { get; set; }
        public string Modelo { get; set; }
        public bool KM { get; set; }
        private bool _Alugado;
        [Ignore]
        public ObservableRangeCollection<AluguelGasto> Gastos { get; set; }
        [Ignore]
        public ObservableRangeCollection<Reabastecimento> Reabastecimentos { get; set; }
        [Ignore]
        public ObservableRangeCollection<AvaliacaoAluguel> Avaliacoes { get; set; }
        [Ignore]
        public Viagem ItemViagem { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? DataRetirada { get; set; }

        public DateTime? DataDevolucao { get; set; }

        public string Descricao { get; set; }

        public int? IdentificadorCarroEventoRetirada { get; set; }

        public int? IdentificadorCarroEventoDevolucao { get; set; }
        [Ignore]

        public CarroEvento ItemCarroEventoRetirada { get; set; }
        [Ignore]

        public CarroEvento ItemCarroEventoDevolucao { get; set; }
        [Ignore]

        public ObservableRangeCollection<CarroDeslocamento> Deslocamentos { get; set; }

        public bool Alugado
        {
            get
            {
                return _Alugado;
            }

            set
            {
                SetProperty(ref _Alugado, value);
            }
        }

        public Carro Clone()
        {
            return (Carro)this.MemberwiseClone();
        }
    }
}
