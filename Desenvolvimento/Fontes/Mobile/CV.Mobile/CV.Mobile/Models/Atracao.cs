using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Atracao: ObservableObject
    {
        private string _Nome;
        private string _CodigoPlace;
        private decimal? _Latitude;
        private decimal? _Longitude;
        private DateTime? _Chegada;
        private DateTime? _Partida;
        private string _Tipo;
        private int? _IdAtracaoPai;
        private int? _IdentificadorAtracaoPai;


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

        public decimal? Latitude
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

        public decimal? Longitude
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
    }
}
