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
    }
}
