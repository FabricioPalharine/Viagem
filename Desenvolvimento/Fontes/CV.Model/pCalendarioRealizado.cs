using CV.Model.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class CalendarioRealizado
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public DateTime DataInicio { get; set; } 
        public DateTime DataFim { get; set; }

        public string Nome { get; set; }

        public string Complemento { get; set; }
        public string Titulo
        {
            get
            {
                string Texto = string.Empty;
                if (Tipo == "A")
                    Texto = String.Format(MensagemModelo.AtracaoVisita, Nome);
                if (Tipo == "R")
                    Texto = String.Format(MensagemModelo.RefeicaoFazendo, Complemento ?? string.Empty, Nome);
                if (Tipo == "HCI")
                    Texto = String.Format(MensagemModelo.HotelCheckIn, Nome);
                if (Tipo == "HCO")
                    Texto = String.Format(MensagemModelo.HotelCheckOut, Nome);
                if (Tipo == "CR")
                    Texto = String.Format(MensagemModelo.CarroRetirada, Nome, Complemento ?? string.Empty);
                if (Tipo == "CD")
                    Texto = String.Format(MensagemModelo.CarroDevolucao, Nome, Complemento ?? string.Empty);
                if (Tipo == "DC")
                    Texto = String.Format(MensagemModelo.CarroDeslocamento, Nome);
                if (Tipo == "RC")
                    Texto = String.Format(MensagemModelo.CarroReabastecido, Nome);
                if (Tipo == "L")
                    Texto = String.Format(MensagemModelo.LojaCompra, Nome);
                if (Tipo == "VO")
                    Texto = String.Format(MensagemModelo.ViagemAereaOrigem, Nome, Complemento ?? string.Empty);
                if (Tipo == "VD")
                    Texto = String.Format(MensagemModelo.ViagemAereaDestino, Nome, Complemento ?? string.Empty);
                if (Tipo == "VE")
                    Texto = String.Format(MensagemModelo.ViagemAereaEscala, Nome, Complemento ?? string.Empty);
                if (Tipo == "VV")
                    Texto = String.Format(MensagemModelo.ViagemAereaViajando, Nome);

                return Texto;
            }
        }
    }
}
