using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Posicao: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? DataGMT { get; set; }
        public double? Velocidade { get; set; }
        public DateTime? DataLocal { get; set; }
    }
}
