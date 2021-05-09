
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ConsultaAmigo: ObservableObject
    {
        private int? _IdentificadorUsuario;
        private string _EMail;
        private string _Nome;
        private bool _Seguidor;
        private bool _Seguido;

        public int Acao { get; set; }

        public bool Seguidor
        {
            get
            {
                return _Seguidor;
            }

            set
            {
                SetProperty(ref _Seguidor , value);
            }
        }

        public bool Seguido
        {
            get
            {
                return _Seguido;
            }

            set
            {
                SetProperty(ref _Seguido, value);
            }
        }

        public string EMail
        {
            get
            {
                return _EMail;
            }

            set
            {
                SetProperty(ref _EMail, value);
                OnPropertyChanged("MailNaoLocalizado");
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
                OnPropertyChanged("UsuarioLocalizado");
                OnPropertyChanged("MailNaoLocalizado");

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

        public bool UsuarioLocalizado
        {
            get
            {
                return IdentificadorUsuario.HasValue;
            }
        }

        public bool MailNaoLocalizado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(EMail) && !IdentificadorUsuario.HasValue;
            }
        }
    }
}
