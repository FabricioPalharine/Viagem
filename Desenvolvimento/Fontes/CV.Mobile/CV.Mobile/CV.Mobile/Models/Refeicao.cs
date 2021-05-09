
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Refeicao : ObservableObject
    {
        private string _Nome;
        private string _CodigoPlace;
        private double? _Latitude;
        private double? _Longitude;
        private DateTime? _Data;
        private DateTime? _DataTermino;
        private string _Tipo;
        private int? _IdentificadorAtracaoPai;
        private TimeSpan? _Hora;
        private TimeSpan? _HoraTermino;

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

        public DateTime? Data
        {
            get
            {
                return _Data;
            }

            set
            {
                SetProperty(ref _Data, value);
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

    

        public int? IdentificadorAtracao
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

        public TimeSpan? Hora
        {
            get
            {
                return _Hora;
            }

            set
            {
                SetProperty(ref _Hora, value);
            }
        }

        public DateTime? DataTermino
        {
            get
            {
                return _DataTermino;
            }

            set
            {
                SetProperty(ref _DataTermino, value);
            }
        }

        public TimeSpan? HoraTermino
        {
            get
            {
                return _HoraTermino;
            }

            set
            {
                SetProperty(ref _HoraTermino, value);
            }
        }

        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }
        [Ignore]
        public ObservableCollection<RefeicaoPedido> Pedidos { get; set; }
        [Ignore]
        public ObservableCollection<GastoRefeicao> Gastos { get; set; }
        [Ignore]
        public ObservableCollection<FotoRefeicao> Fotos { get; set; }
        [Ignore]
        public bool Selecionado { get; set; }

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