using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Gasto: ObservableObject
    {
        private string _Descricao;
        private decimal? _Latitude;
        private decimal? _Longitude;
        private DateTime? _Data;
        private decimal? _Valor;
        private bool _Especie;
        private int? _Moeda;
        private DateTime? _DataPagamento;
        private bool _Dividido;
        private bool _ApenasBaixa;
        private ObservableRangeCollection<GastoDividido> _Usuarios = new ObservableRangeCollection<GastoDividido>();




        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public Usuario ItemUsuario { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string Descricao
        {
            get
            {
                return _Descricao;
            }

            set
            {
                SetProperty(ref _Descricao, value);
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

        public ObservableRangeCollection<GastoDividido> Usuarios
        {
            get
            {
                return _Usuarios;
            }

            set
            {
                _Usuarios = value;
            }
        }

        public decimal? Valor
        {
            get
            {
                return _Valor;
            }

            set
            {
                SetProperty(ref _Valor, value);
            }
        }

        public bool Especie
        {
            get
            {
                return _Especie;
            }

            set
            {
                SetProperty(ref _Especie, value);
            }
        }

        public int? Moeda
        {
            get
            {
                return _Moeda;
            }

            set
            {
                SetProperty(ref _Moeda, value);
            }
        }

        public DateTime? DataPagamento
        {
            get
            {
                return _DataPagamento;
            }

            set
            {
                SetProperty(ref _DataPagamento, value);
            }
        }

        public bool Dividido
        {
            get
            {
                return _Dividido;
            }

            set
            {
                SetProperty(ref _Dividido, value);
            }
        }

        public bool ApenasBaixa
        {
            get
            {
                return _ApenasBaixa;
            }

            set
            {
                SetProperty(ref _ApenasBaixa, value);
            }
        }
    }
}
