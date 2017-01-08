using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace CV.Mobile.Models
{
    public class Atracao: ObservableObject
    {
        private string _Nome;
        private string _CodigoPlace;
        private double? _Latitude;
        private double? _Longitude;
        private DateTime? _Chegada;
        private DateTime? _Partida;
        private string _Tipo;
        private int? _IdAtracaoPai;
        private int? _IdentificadorAtracaoPai;
        private TimeSpan? _HoraChegada;
        private TimeSpan? _HoraPartida;

        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string Nome
        {
            get
            {
                return _Nome;
            }

            set
            {
                SetProperty(ref _Nome, value);
            }
        }

        public string CodigoPlace
        {
            get
            {
                return _CodigoPlace;
            }

            set
            {
                SetProperty(ref _CodigoPlace, value);
            }
        }

        public double? Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                SetProperty(ref _Latitude, value);
            }
        }

        public double? Longitude
        {
            get
            {
                return _Longitude;
            }

            set
            {
                SetProperty(ref _Longitude, value);
            }
        }

        public DateTime? Chegada
        {
            get
            {
                return _Chegada;
            }

            set
            {
                SetProperty(ref _Chegada, value);
            }
        }

        public DateTime? Partida
        {
            get
            {
                return _Partida;
            }

            set
            {
                SetProperty(ref _Partida, value);
            }
        }

        public string Tipo
        {
            get
            {
                return _Tipo;
            }

            set
            {
                SetProperty(ref _Tipo, value);
            }
        }

        public int? IdAtracaoPai
        {
            get
            {
                return _IdAtracaoPai;
            }

            set
            {
                SetProperty(ref _IdAtracaoPai, value);
            }
        }

        public int? IdentificadorAtracaoPai
        {
            get
            {
                return _IdentificadorAtracaoPai;
            }

            set
            {
                SetProperty(ref _IdentificadorAtracaoPai, value);
            }
        }

        public TimeSpan? HoraChegada
        {
            get
            {
                return _HoraChegada;
            }

            set
            {
                SetProperty(ref _HoraChegada, value);
            }
        }

        public TimeSpan? HoraPartida
        {
            get
            {
                return _HoraPartida;
            }

            set
            {
                SetProperty(ref _HoraPartida, value);
            }
        }
        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }
        [Ignore]
        public ObservableCollection<AvaliacaoAtracao> Avaliacoes { get; set; }
        [Ignore]
        public ObservableCollection<GastoAtracao> Gastos { get; set; }
        [Ignore]
        public ObservableCollection<FotoAtracao> Fotos { get; set; }

    }
}
