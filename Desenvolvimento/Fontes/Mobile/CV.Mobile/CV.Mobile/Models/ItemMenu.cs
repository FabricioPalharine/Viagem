using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ItemMenu: ObservableObject
    {
        private string _Title;
        private string _IconSource;
        private bool _Visible;
        private int _Codigo;
        private bool _ApenasParticipante;
        private bool _ApenasAmigo;
        private bool _ViagemAberta = true;

        public string Title
        {
            get
            {
                return _Title;
            }

            set
            {
                SetProperty(ref _Title, value);
            }
        }

        public string IconSource
        {
            get
            {
                return _IconSource;
            }

            set
            {
                SetProperty(ref _IconSource, value);
            }
        }

        public bool Visible
        {
            get
            {
                return _Visible;
            }

            set
            {
                SetProperty(ref _Visible, value);
            }
        }

        public int Codigo
        {
            get
            {
                return _Codigo;
            }

            set
            {
                SetProperty(ref _Codigo, value);
            }
        }

        public bool ApenasParticipante
        {
            get
            {
                return _ApenasParticipante;
            }

            set
            {
                _ApenasParticipante = value;
            }
        }

        public bool ApenasAmigo
        {
            get
            {
                return _ApenasAmigo;
            }

            set
            {
                _ApenasAmigo = value;
            }
        }

        public bool ViagemAberta
        {
            get
            {
                return _ViagemAberta;
            }

            set
            {
                _ViagemAberta = value;
            }
        }
    }
}
