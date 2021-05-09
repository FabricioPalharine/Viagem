
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class GastoCompra : ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }

        public int? IdentificadorGasto { get; set; }

        public int? IdentificadorLoja { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
        [Ignore]
        public Gasto ItemGasto { get; set; }
        [Ignore]
        public ObservableCollection<ItemCompra> ItensComprados { get; set; }
        private bool _Atualizado = true;

        public bool AtualizadoBanco
        {
            get
            {
                return _Atualizado;
            }

            set
            {
                _Atualizado = value;
            }
        }
    }
}
