using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Viagem: ObservableObject
    {
        private int? _Id;
        private int? _Identificador;
        private int? _IdentificadorUsuario;
        private string _Nome;
        private DateTime? _DataInicio;
        private DateTime? _DataFim;
        private bool _Aberto;
        private bool _UnidadeMetrica;
        private int? _QuantidadeParticipantes;
        private bool _PublicaGasto;
        private decimal? _PercentualIOF;
        private int? _Moeda;
        private bool _Edicao;
        private bool _VejoGastos;

        public int? Id
        {
            get
            {
                return _Id;
            }

            set
            {
                SetProperty(ref _Id, value);
            }
        }

        public int? Identificador
        {
            get
            {
                return _Identificador;
            }

            set
            {
                SetProperty(ref _Identificador, value);

            }
        }

        public int? IdentificadorUsuario
        {
            get
            {
                return _IdentificadorUsuario;
            }

            set
            {
                SetProperty(ref _IdentificadorUsuario, value);
            }
        }

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

        public DateTime? DataInicio
        {
            get
            {
                return _DataInicio;
            }

            set
            {
                SetProperty(ref _DataInicio, value);
            }
        }

        public DateTime? DataFim
        {
            get
            {
                return _DataFim;
            }

            set
            {
                SetProperty(ref _DataFim, value);
            }
        }

        public bool Aberto
        {
            get
            {
                return _Aberto;
            }

            set
            {
                SetProperty(ref _Aberto, value);
            }
        }

        public bool UnidadeMetrica
        {
            get
            {
                return _UnidadeMetrica;
            }

            set
            {
                SetProperty(ref _UnidadeMetrica, value);
            }
        }

        public int? QuantidadeParticipantes
        {
            get
            {
                return _QuantidadeParticipantes;
            }

            set
            {
                SetProperty(ref _QuantidadeParticipantes, value);
            }
        }

        public bool PublicaGasto
        {
            get
            {
                return _PublicaGasto;
            }

            set
            {
                SetProperty(ref _PublicaGasto, value);
            }
        }

        public decimal? PercentualIOF
        {
            get
            {
                return _PercentualIOF;
            }

            set
            {
                SetProperty(ref _PercentualIOF, value);
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

        public bool Edicao
        {
            get
            {
                return _Edicao;
            }

            set
            {
                _Edicao = value;
            }
        }

        public bool VejoGastos
        {
            get
            {
                return _VejoGastos;
            }

            set
            {
                _VejoGastos = value;
            }
        }
    }
}
