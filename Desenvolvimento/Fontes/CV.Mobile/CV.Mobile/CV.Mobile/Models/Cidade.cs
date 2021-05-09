
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Cidade : ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        private bool _Selecionada;
        private bool _Visivel = true;
        public int? Identificador { get; set; }
        public string Nome { get; set; }
        public string Estado { get; set; }
        public string NomePais { get; set; }
        public string Tipo { get; set; }
        [Ignore]
        public bool Selecionada
        {
            get
            {
                return _Selecionada;
            }

            set
            {
                SetProperty(ref _Selecionada, value);
            }
        }
        [Ignore]
        public bool Visivel
        {
            get
            {
                return _Visivel;
            }

            set
            {
                SetProperty(ref _Visivel, value);
            }
        }
    }
}
