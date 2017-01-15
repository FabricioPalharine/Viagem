using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class DataCalendario: ObservableObject
    {
        private DateTime _Data;

        public DateTime Data
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
    }
}
