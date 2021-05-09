

using CV.Mobile.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Gasto: ObservableObject
    {
        private string _Descricao;
        private double? _Latitude;
        private double? _Longitude;
        private DateTime? _Data;
        private decimal? _Valor;
        private bool _Especie;
        private int? _Moeda;
        private DateTime? _DataPagamento;
        private bool _Dividido;
        private bool _ApenasBaixa;
        private ObservableCollection<GastoDividido> _Usuarios = new ObservableCollection<GastoDividido>();
        private TimeSpan? _Hora;


        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        [Ignore]
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
        [Ignore]

        public ObservableCollection<GastoDividido> Usuarios 
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
        [Ignore]

        public string SiglaMoeda
        {
            get
            {
                if (Moeda.HasValue)
                    return ((enumMoeda)Moeda).ToString();
                else
                    return null;
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
        [Ignore]

        public ObservableCollection<GastoAtracao> Atracoes { get; set; } = new ObservableCollection<GastoAtracao>();
        [Ignore]

        public ObservableCollection<GastoHotel> Hoteis { get; set; } = new ObservableCollection<GastoHotel>();
        [Ignore]

        public ObservableCollection<GastoCompra> Compras { get; set; } = new ObservableCollection<GastoCompra>();
        [Ignore]

        public ObservableCollection<GastoRefeicao> Refeicoes { get; set; } = new ObservableCollection<GastoRefeicao>();
        [Ignore]

        public ObservableCollection<AluguelGasto> Alugueis { get; set; } = new ObservableCollection<AluguelGasto>();
        [Ignore]

        public ObservableCollection<GastoViagemAerea> ViagenAereas { get; set; } = new ObservableCollection<GastoViagemAerea>();

        [Ignore]
        public ObservableCollection<ReabastecimentoGasto> Reabastecimentos { get; set; } = new ObservableCollection<ReabastecimentoGasto>();
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
