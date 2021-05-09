
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _CodigoAlbum;
        private bool _Edicao;
        private bool _VejoGastos;
        private DateTime? _DataAlteracao;
        private DateTime? _DataExclusao;
        private ObservableCollection<ParticipanteViagem> _Participantes = new ObservableCollection<ParticipanteViagem>();
        private ObservableCollection<UsuarioGasto> _UsuariosGastos = new ObservableCollection<UsuarioGasto>();
        private bool _ControlaPosicaoGPS;
        private string _ShareToken;

        [PrimaryKey, AutoIncrement]

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

        public string CodigoAlbum
        {
            get
            {
                return _CodigoAlbum;
            }

            set
            {
                SetProperty(ref _CodigoAlbum, value);
            }
        }


        public string ShareToken
        {
            get
            {
                return _ShareToken;
            }

            set
            {
                SetProperty(ref _ShareToken, value);
            }
        }

        public DateTime? DataAlteracao
        {
            get
            {
                return _DataAlteracao;
            }

            set
            {
                _DataAlteracao = value;
            }
        }

        public DateTime? DataExclusao
        {
            get
            {
                return _DataExclusao;
            }

            set
            {
                _DataExclusao = value;
            }
        }
        [Ignore]
        public ObservableCollection<ParticipanteViagem> Participantes
        {
            get
            {
                return _Participantes;
            }

            set
            {
                SetProperty(ref _Participantes, value);
            }
        }
        [Ignore]
        public ObservableCollection<UsuarioGasto> UsuariosGastos
        {
            get
            {
                return _UsuariosGastos;
            }

            set
            {
                _UsuariosGastos = value;
            }
        }

        public bool ControlaPosicaoGPS
        {
            get
            {
                return _ControlaPosicaoGPS;
            }

            set
            {
                _ControlaPosicaoGPS = value;
            }
        }
    }
}
