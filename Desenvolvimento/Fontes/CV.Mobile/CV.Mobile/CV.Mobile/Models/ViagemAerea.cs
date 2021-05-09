
using CV.Mobile.Enums;
using CV.Mobile.Helper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private decimal? _Distancia;
        public decimal? Distancia
        {
            get
            {
                return _Distancia;
            }

            set
            {
                SetProperty(ref _Distancia, value);
            }
        }

        public DateTime? DataPrevista { get; set; }
        [Ignore]
        public ObservableCollection<GastoViagemAerea> Gastos { get; set; }
        [Ignore]
        public ObservableCollection<ViagemAereaAeroporto> Aeroportos { get; set; }
        [Ignore]
        public ObservableCollection<AvaliacaoAerea> Avaliacoes { get; set; }

        [Ignore]
        public DateTime? DataInicio
        {
            get
            {
                DateTime? data = null;
                var itemInicio = Aeroportos?.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).FirstOrDefault();
                if (itemInicio != null && itemInicio.DataChegada.HasValue)
                    data = itemInicio.DataChegada;
                return data;

            }
        }

        [Ignore]
        public DateTime? DataFim
        {
            get
            {
                DateTime? data = null;
                var itemInicio = Aeroportos?.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).FirstOrDefault();
                if (itemInicio != null && itemInicio.DataChegada.HasValue)
                    data = itemInicio.DataPartida.HasValue?itemInicio.DataPartida: itemInicio.DataChegada;
                return data;

            }
        }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public int? Tipo { get; set; }

        public string Descricao { get; set; }
        private bool _Atualizado = true;

       [Ignore]
       public string DescricaoTipo
        {
            get
            {
                return Tipo.HasValue ? ((enumTipoTransporte)Tipo).Descricao() : null;
            }
        }

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
