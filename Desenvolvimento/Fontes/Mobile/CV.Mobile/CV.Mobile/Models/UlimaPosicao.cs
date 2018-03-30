using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class UlimaPosicao : ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime? DataUltimaPosicao { get; set; }
    }
}
