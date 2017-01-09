using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Loja: ObservableObject
    {
        private string _Nome;
        private string _CodigoPlace;
        private double? _Latitude;
        private double? _Longitude;

        private int? _IdentificadorAtracaoPai;


        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string Nome
        {
            get
            {
                return _Nome;
            }

            set
            {
                SetProperty(ref _Nome, value);
            }
        }

        public string CodigoPlace
        {
            get
            {
                return _CodigoPlace;
            }

            set
            {
                SetProperty(ref _CodigoPlace, value);
            }
        }

        public double? Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                SetProperty(ref _Latitude, value);
            }
        }

        public double? Longitude
        {
            get
            {
                return _Longitude;
            }

            set
            {
                SetProperty(ref _Longitude, value);
            }
        }
                

        public int? IdentificadorAtracao
        {
            get
            {
                return _IdentificadorAtracaoPai;
            }

            set
            {
                SetProperty(ref _IdentificadorAtracaoPai, value);
            }
        }

      

        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }
        [Ignore]
        public ObservableRangeCollection<GastoCompra> Compras { get; set; }
        [Ignore]
        public ObservableRangeCollection<AvaliacaoLoja> Avaliacoes { get; set; }

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
