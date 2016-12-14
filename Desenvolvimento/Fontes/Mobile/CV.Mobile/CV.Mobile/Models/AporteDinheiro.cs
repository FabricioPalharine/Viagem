using CV.Mobile.Helpers;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class AporteDinheiro: ObservableObject
    {
        private decimal? _Valor;
        private int? _Moeda;
        private DateTime? _DataAporte;
        private decimal? _Cotacao;
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }

        public decimal? Valor
        {
            get
            {
                return _Valor;
            }

            set
            {
                SetProperty(ref _Valor, value);
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
                OnPropertyChanged("MoedaSigla");
            }
        }

        public DateTime? DataAporte
        {
            get
            {
                return _DataAporte;
            }

            set
            {
                SetProperty(ref _DataAporte, value);
            }
        }

        public decimal? Cotacao
        {
            get
            {
                return _Cotacao;
            }

            set
            {
                SetProperty(ref _Cotacao, value);
            }
        }

        public string NomeUsuario { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }

        public int? IdGasto { get; set; }
        public int? IdentificadorGasto { get; set; }
        public Gasto ItemGasto { get; set; }
        public Usuario ItemUsuario { get; set; }
        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }
    }
}
