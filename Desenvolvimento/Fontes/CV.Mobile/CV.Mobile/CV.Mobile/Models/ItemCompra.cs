﻿
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ItemCompra: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }

        public int? IdentificadorGastoCompra { get; set; }

        private int? _IdentificadorListaCompra;

        private string _Descricao;

        private string _Marca;
        public decimal? Valor { get; set; }

        public bool? Reembolsavel { get; set; }

        private string _Destinatario;
        [Ignore]
        public GastoCompra ItemGastoCompra { get; set; }
     
        private int? _IdentificadorUsuario;
        [Ignore]
        public Usuario ItemUsuario { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
        public string NomeUsuario { get; set; }

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

        public int? IdentificadorUsuario
        {
            get
            {
                return _IdentificadorUsuario;
            }

            set
            {
                SetProperty(ref _IdentificadorUsuario, value);
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

        public int? IdentificadorListaCompra
        {
            get
            {
                return _IdentificadorListaCompra;
            }

            set
            {
                SetProperty(ref _IdentificadorListaCompra, value);
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
        public ItemCompra Clone()
        {
            return (ItemCompra)this.MemberwiseClone();
        }
    }
}
