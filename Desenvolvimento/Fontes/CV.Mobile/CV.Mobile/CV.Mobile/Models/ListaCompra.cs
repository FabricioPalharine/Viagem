

using CV.Mobile.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ListaCompra: ObservableObject
    {

        private int? _IdentificadorUsuario;
        private int? _IdentificadorUsuarioPedido;
        private string _Descricao;
        private string _Marca;
        private decimal? _ValorMaximo;
        private int? _Moeda;
        private bool _Reembolsavel;
        private string _Destinatario;
        private int? _Status;

        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? IdViagem { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }

        public int? IdentificadorUsuario
        {
            get
            {
                return _IdentificadorUsuario;
            }

            set
            {
               SetProperty(ref _IdentificadorUsuario , value);
            }
        }

        public int? IdentificadorUsuarioPedido
        {
            get
            {
                return _IdentificadorUsuarioPedido;
            }

            set
            {
                SetProperty(ref _IdentificadorUsuarioPedido, value);
            }
        }

        public string Descricao
        {
            get
            {
                return _Descricao;
            }

            set
            {
                SetProperty(ref _Descricao, value);
            }
        }

        public string Marca
        {
            get
            {
                return _Marca;
            }

            set
            {
                SetProperty(ref _Marca, value);
            }
        }

        public decimal? ValorMaximo
        {
            get
            {
                return _ValorMaximo;
            }

            set
            {
                SetProperty(ref _ValorMaximo, value);
            }
        }

        public int? Moeda
        {
            get
            {
                return _Moeda;
            }

            set
            {
                SetProperty(ref _Moeda, value);
            }
        }

        public bool Reembolsavel
        {
            get
            {
                return _Reembolsavel;
            }

            set
            {
                SetProperty(ref _Reembolsavel, value);
            }
        }

        public string Destinatario
        {
            get
            {
                return _Destinatario;
            }

            set
            {
                SetProperty(ref _Destinatario, value);
            }
        }

        public int? Status
        {
            get
            {
                return _Status;
            }

            set
            {
                SetProperty(ref _Status, value);
            }
        }

        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        [Ignore]
        public Usuario ItemUsuario { get; set; }
        [Ignore]
        public Usuario ItemUsuarioPedido { get; set; }

        public string NomeUsuario { get; set; }
        public string NomeUsuarioPedido { get; set; }

        [Ignore]
        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }
        [Ignore]
        public string DestinatarioSelecao
        {
            get
            {
                if (IdentificadorUsuarioPedido.HasValue )
                    return NomeUsuarioPedido;
                else
                    return Destinatario;
            }
        }
        [Ignore]
        public string Comprado
        {
            get
            {
                return (Status.GetValueOrDefault(-1) == (int)enumStatusListaCompra.Comprado) ?"Sim":"Não";
            }
        }
        [Ignore]
        public string DescricaoCombinada
        {
            get
            {
                string _Descricao = String.Format("{0} - {1}", Descricao, Marca);

                return _Descricao;
            }
        }
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
