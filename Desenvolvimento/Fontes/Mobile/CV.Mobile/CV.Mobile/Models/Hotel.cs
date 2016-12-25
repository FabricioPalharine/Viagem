using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Hotel: ObservableObject
    {
        public int? Id { get; set; }

        public int? Identificador { get; set; }

       public int? IdentificadorViagem { get; set; }

        public int? IdentificadorCidade { get; set; }

        public string Nome { get; set; }
    
        public string CodigoPlace { get; set; }

        private DateTime? _DataEntrada;

        private DateTime? _DataSaidia;

        public Double? Longitude { get; set; }

        public Double? Latitude { get; set; }

        public DateTime? EntradaPrevista { get; set; }

        public DateTime? SaidaPrevista { get; set; }

        public Cidade ItemCidade { get; set; }

        public ObservableRangeCollection<FotoHotel> Fotos { get; set; }

        public ObservableRangeCollection<GastoHotel> Gastos { get; set; }

        public ObservableRangeCollection<HotelAvaliacao> Avaliacoes { get; set; }

        public Viagem ItemViagem { get; set; }

        public IList<HotelEvento> Eventos { get; set; }

        public int? Raio { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? DataEntrada
        {
            get
            {
                return _DataEntrada;
            }

            set
            {
              SetProperty(ref   _DataEntrada , value);
            }
        }

        public DateTime? DataSaidia
        {
            get
            {
                return _DataSaidia;
            }

            set
            {
                SetProperty(ref _DataSaidia, value);

            }
        }

        public TimeSpan? HoraEntrada
        {
            get
            {
                return _HoraEntrada;
            }

            set
            {
                SetProperty(ref _HoraEntrada, value);
            }
        }

        public TimeSpan? HoraSaida
        {
            get
            {
                return _HoraSaida;
            }

            set
            {
                SetProperty(ref _HoraSaida, value);
            }
        }

        private TimeSpan? _HoraEntrada;
        private TimeSpan? _HoraSaida;

    }
}
