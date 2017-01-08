using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class RefeicaoPedido : ObservableObject
    {
        private string _Comentario;
        private int? _Nota;
        private string _Pedido;

        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorRefeicao { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        [Ignore]
        public Usuario ItemUsuario { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }

        public string Comentario
        {
            get
            {
                return _Comentario;
            }

            set
            {
                SetProperty(ref _Comentario, value);
            }
        }

        public int? Nota
        {
            get
            {
                return _Nota;
            }

            set
            {
                SetProperty(ref _Nota, value);
            }
        }

        public string Pedido
        {
            get
            {
                return _Pedido;
            }

            set
            {
                SetProperty(ref _Pedido, value);
            }
        }
    }
}
