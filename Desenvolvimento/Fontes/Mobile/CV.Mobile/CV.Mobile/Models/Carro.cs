using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Carro: ObservableObject
    {

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public string Locadora { get; set; }
        public string Modelo { get; set; }
        public bool KM { get; set; }
        private bool _Alugado;

        public ObservableRangeCollection<AluguelGasto> Gastos { get; set; }

        public ObservableRangeCollection<Reabastecimento> Reabastecimentos { get; set; }

        public ObservableRangeCollection<AvaliacaoAluguel> Avaliacoes { get; set; }

        public Viagem ItemViagem { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? DataRetirada { get; set; }

        public DateTime? DataDevolucao { get; set; }

        public string Descricao { get; set; }

        public int? IdentificadorCarroEventoRetirada { get; set; }

        public int? IdentificadorCarroEventoDevolucao { get; set; }

        public CarroEvento ItemCarroEventoRetirada { get; set; }

        public CarroEvento ItemCarroEventoDevolucao { get; set; }

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
