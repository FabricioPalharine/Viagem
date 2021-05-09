using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Models
{
    public class PosicaoMapa: ObservableObject
    {
        
        private double _latitude;
        private double _longitude;
        public string NomeMensagem
        {
            get; set;
        }
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }
    }
}
