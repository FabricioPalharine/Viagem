using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Timeline
    {
        public string Tipo { get; set; }
        public string Comentario { get; set; }
        public string Texto { get; set; }
        public string Url { get; set; }
        public DateTime? Data { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string GoogleId { get; set; }

        public int? Identificador { get; set; }
        public IEnumerable<UsuarioConsulta> Usuarios { get; set; }

        public string URLMapa
        {
            get
            {
                string url = null;
                if (Latitude.GetValueOrDefault(0) != 0 || Longitude.GetValueOrDefault(0) != 0)
                    url = "https://maps.googleapis.com/maps/api/staticmap?center=" + Latitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) + "," + Longitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) +
                  "&zoom=16&size=400x400&maptype=roadmap&markers=color:blue%7C" + Latitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) + "," + Longitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) +
                  "&key=" + GlobalSetting.ClientAPI;
                return url;
            }
        }

        public string UrlCompleta
        {
            get { return Url ?? string.Empty; }
        }

        public bool TemPosicao
        {
            get
            {
                return Latitude.GetValueOrDefault(0) != 0 || Longitude.GetValueOrDefault(0) != 0;
            }
        }

        public bool LinhaRefeicao
        {
            get
            {
                return Tipo == "Refeicao";
            }
        }

        public string UrlThumbnail { get; set; }

        public bool LinhaComentario
        {
            get
            {
                return Tipo == "Comentario" || Tipo == "Foto" || Tipo == "Video";
            }
        }

        public bool LinhaFoto
        {
            get
            {
                return Tipo == "Foto";
            }
        }
        public bool LinhaVideo
        {
            get
            {
                return Tipo == "Video";
            }
        }

        public bool ExibeImagem
        {
            get
            {
                bool exibeimagem = Tipo == "Foto" || Tipo == "Video";
                return exibeimagem;
            }
        }

        public string TextoCompleto
        {
            get
            {
                string ValorTexto = string.Empty;
                if (Tipo != "Reabastecimento")
                {
                    ValorTexto = String.Join(", ", Usuarios.Select(d => d.Nome).ToArray());
                    if (ValorTexto.LastIndexOf(", ") > -1)
                    {
                        int Posicao = ValorTexto.LastIndexOf(", ");
                        ValorTexto = ValorTexto.Remove(Posicao, 2);
                        ValorTexto = ValorTexto.Insert(Posicao, " e ");
                    }
                    ValorTexto += " ";
                }
                if (Tipo == "AtracaoChegada")
                    ValorTexto += (Usuarios.Count() == 1 ? "chegou na atração " : "chegaram na atração ") + Texto;
                else if (Tipo == "AtracaoPartida")
                    ValorTexto += (Usuarios.Count() == 1 ? "deixou a atração " : "deixaram a atração ") + Texto;
                else if (Tipo == "AtracaoVisita")
                    ValorTexto += (Usuarios.Count() == 1 ? "visitou a atração " : "visitaram a atração ") + Texto;
                else if (Tipo == "Refeicao")
                    ValorTexto += (Usuarios.Count() == 1 ? "comeu" : "comeram") + " no restaurante " + Texto;
                else if (Tipo == "HotelCheckIn")
                    ValorTexto += (Usuarios.Count() == 1 ? "fez" : "fizeram") + " check in na hospedagem " + Texto;
                else if (Tipo == "HotelChekckOut")
                    ValorTexto += (Usuarios.Count() == 1 ? "fez" : "fizeram") + " check out na hospedagem " + Texto;
                else if (Tipo == "HotelEntrada")
                    ValorTexto += (Usuarios.Count() == 1 ? "chegou" : "chegaram") + " na hospedagem " + Texto;
                else if (Tipo == "HotelSaida")
                    ValorTexto += (Usuarios.Count() == 1 ? "deixou" : "deixaram") + " a hospedagem " + Texto;
                else if (Tipo == "Comentario")
                    ValorTexto += " comentou";
                else if (Tipo == "Foto")
                    ValorTexto += " tirou a foto";
                else if (Tipo == "Video")
                    ValorTexto += " gravou o vídeo";
                else if (Tipo == "DeslocamentoChegadaOrigem")
                    ValorTexto += (Usuarios.Count() == 1 ? "chegou em " : "chegaram em ")  + Comentario + " para iniciar a viagem " + Texto + (string.IsNullOrEmpty(Url)?"":  " pela companhia " + Url);
                else if (Tipo == "DeslocamentoPartidaOrigem")
                    ValorTexto += (Usuarios.Count() == 1 ? "partiu de " : "partiram de ") + Comentario + " para iniciar a viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);
                else if (Tipo == "DeslocamentoOrigem")
                    ValorTexto += (Usuarios.Count() == 1 ? "inciou" : "iniciaram") + " a viagem " + Texto + " em " + Comentario + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);

                else if (Tipo == "DeslocamentoChegadaDestino")
                    ValorTexto += (Usuarios.Count() == 1 ? "chegou em " : "chegaram em ") + Comentario + " para concluir a viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);
                else if (Tipo == "DeslocamentoPartidaDestino")
                    ValorTexto += (Usuarios.Count() == 1 ? "partiu de " : "partiram de ") + Comentario + " para concluir a viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);
                else if (Tipo == "DeslocamentoDestino")
                    ValorTexto += (Usuarios.Count() == 1 ? "terminou" : "terminaram") + " a viagem " + Texto + " em " + Comentario + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);

                else if (Tipo == "DeslocamentoChegadaEscala")
                    ValorTexto += (Usuarios.Count() == 1 ? "chegou em " : "chegaram em ") + Comentario + " para fazer parada na viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);
                else if (Tipo == "DeslocamentoPartidaEscala")
                    ValorTexto += (Usuarios.Count() == 1 ? "partiu de " : "partiram de ") + Comentario + " para fazer parada na viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);
                else if (Tipo == "DeslocamentoEscala")
                    ValorTexto += (Usuarios.Count() == 1 ? "fez" : "fizeram") + " parada em " + Comentario + " na viagem " + Texto + (string.IsNullOrEmpty(Url) ? "" : " pela companhia " + Url);

                return ValorTexto;
            }
        }

        public bool TemAvaliacoes
        {
            get
            {
                return Avaliacoes.Any();
            }
        }

        public List<UsuarioConsulta> Avaliacoes
        {
            get
            {
                return Usuarios.Where(d => d.Nota.HasValue).ToList();
            }
        }

        public double AvaliacoesHeight
        {
            get
            {
                return Avaliacoes.Count * 18;
            }
        }

        public double UsuariosHeight
        {
            get
            {
                return Usuarios.Count() * 18;
            }
        }

    }
}
