using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Sugestao: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        private string _Local;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Comentario { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorCidade { get; set; }

        public int? IdentificadorUsuario { get; set; }
        private int? _Status;
        [Ignore]
        public Usuario ItemUsuario { get; set; }       
        public string Tipo { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string CodigoPlace { get; set; }
        public string NomeUsuario { get; set; }
        public string NomeCidade { get; set; }

        public int? Status
        {
            get
            {
                return _Status;
            }

            set
            {
                SetProperty(ref _Status, value);
                OnPropertyChanged("PermiteAcao");
            }
        }
        [Ignore]
        public bool PermiteAcao
        {
            get
            {
                return _Status <= 1;
            }
        }

        public string Local
        {
            get
            {
                return _Local;
            }

            set
            {
                _Local = value;
                OnPropertyChanged();
            }
        }
    }
}
