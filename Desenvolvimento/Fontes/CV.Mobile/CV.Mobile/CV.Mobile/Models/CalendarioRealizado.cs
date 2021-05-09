
using CV.Mobile.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CalendarioRealizado
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public bool PossuiDataFim
        {
            get
            {
                return DataInicio != DataFim && DataFim > new DateTime(1900,1,1);
            }
        }
        public string Nome { get; set; }

        public string Complemento { get; set; }
        public string Titulo
        {
            get
            {
                string Texto = string.Empty;
                if (Tipo == "A")
                    Texto = String.Format(AppResource.AtracaoVisita, Nome);
                if (Tipo == "R")
                    Texto = String.Format(AppResource.RefeicaoFazendo, Complemento ?? string.Empty, Nome);
                if (Tipo == "HCI")
                    Texto = String.Format(AppResource.HotelCheckIn, Nome);
                if (Tipo == "HCO")
                    Texto = String.Format(AppResource.HotelCheckOut, Nome);
                if (Tipo == "CR")
                    Texto = String.Format(AppResource.CarroRetirada, Nome, Complemento ?? string.Empty);
                if (Tipo == "CD")
                    Texto = String.Format(AppResource.CarroDevolucao, Nome, Complemento ?? string.Empty);
                if (Tipo == "DC")
                    Texto = String.Format(AppResource.CarroDeslocamento, Nome);
                if (Tipo == "RC")
                    Texto = String.Format(AppResource.CarroReabastecido, Nome);
                if (Tipo == "L")
                    Texto = String.Format(AppResource.LojaCompra, Nome);
                if (Tipo == "VO")
                    Texto = String.Format(AppResource.ViagemAereaOrigem, Nome, Complemento ?? string.Empty);
                if (Tipo == "VD")
                    Texto = String.Format(AppResource.ViagemAereaDestino, Nome, Complemento ?? string.Empty);
                if (Tipo == "VE")
                    Texto = String.Format(AppResource.ViagemAereaEscala, Nome, Complemento ?? string.Empty);
                if (Tipo == "VV")
                    Texto = String.Format(AppResource.ViagemAereaViajando, Nome);

                return Texto;
            }
        }
    }
}
