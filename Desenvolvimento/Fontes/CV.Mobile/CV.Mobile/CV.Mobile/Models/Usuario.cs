using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SQLite;

namespace CV.Mobile.Models
{
    public class Usuario: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        [Ignore]
        public string EMail { get; set; }
        public string Nome { get; set; }
        [Ignore]
        public string Token { get; set; }
        [Ignore]
        public string RefreshToken { get; set; }
        [Ignore]
        public DateTime? DataToken { get; set; }
        [Ignore]
        public int? Lifetime { get; set; }
        [Ignore]
        public string Codigo { get; set; }
        [Ignore]
        public bool Selecionado
        {
            get
            {
                return _Selecionado;
            }

            set
            {
                SetProperty(ref _Selecionado, value);
            }
        }

        [Ignore]
        public string Complemento
        {
            get
            {
                return _Complemento;
            }

            set
            {
                SetProperty(ref _Complemento, value);

            }
        }

        private bool _Selecionado;
        private string _Complemento;
    }
}
