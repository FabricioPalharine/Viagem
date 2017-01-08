using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class UsuarioGasto : ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        [Ignore]
        public Usuario ItemUsuario { get; set; }

    }
}
