using CV.Business.Library;
using CV.Data;
using CV.Model;
using CV.Model.Results;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CV.Business
{
    public class ConsultaBusiness : BusinessBase
    {
        public List<ExtratoMoeda> ConsultarExtratoMoeda(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime DataInicio)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ConsultarExtratoMoeda(IdentificadorUsuario, IdentificadorViagem, Moeda, DataInicio);
            }
        }

        public List<AjusteGastoDividido> ListarGastosAcerto(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarGastosAcerto(IdentificadorViagem, IdentificadorUsuario, DataInicio, DataFim);
            }
        }

        public List<RelatorioGastos> ListarGastosViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim, string Tipo)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarGastosViagem(IdentificadorViagem, IdentificadorUsuario, DataInicio, DataFim, Tipo);
            }
        }

        public List<Timeline> CarregarTimeline(int? IdentificadorViagem, int? IdentificadorUsuarioConsulta, int? IdentificadorUsuarioEvento, DateTime? DataMaxima, DateTime? DataMinima, int NumeroRegistros, string Tipo, int? Identificador, int? IdentificadorUsuario)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                var lista = data.CarregarTimeline(IdentificadorViagem, IdentificadorUsuarioConsulta, IdentificadorUsuarioEvento, DataMaxima, DataMinima, NumeroRegistros, Tipo, Identificador);
                

                return lista;
            }
        }

        public List<LocaisVisitados> CarregarLocaisVisitados(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.CarregarLocaisVisitados(IdentificadorViagem, DataDe, DataAte, null, null);
            }
        }

        public LocaisVisitados CarregarDetalhesAtracao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle, int IdentificadorUsuarioLogado)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.LocaisFilho = data.CarregarLocaisVisitados(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Detalhes = data.CarregarDetalhesAtracaoVisitada(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosAtracao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosAtracao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle,IdentificadorUsuarioLogado);
                //AjustarLinkFotos(itemRetorno.Fotos, IdentificadorUsuarioLogado);
            }
            return itemRetorno;
        }

        public LocaisVisitados CarregarDetalhesHotel(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle, int IdentificadorUsuarioLogado)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.Detalhes = data.CarregarDetalhesHotelHospedado(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosHotel(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosHotel(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle,IdentificadorUsuarioLogado);
                //AjustarLinkFotos(itemRetorno.Fotos, IdentificadorUsuarioLogado);

            }
            return itemRetorno;
        }

        public LocaisVisitados CarregarDetalhesRefeicao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle, int IdentificadorUsuarioLogado)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.Detalhes = data.CarregarDetalhesRestaurante(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosRefeicao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosRefeicao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle,IdentificadorUsuarioLogado);
               // AjustarLinkFotos(itemRetorno.Fotos, IdentificadorUsuarioLogado);

            }
            return itemRetorno;
        }

        private void AjustarLinkFotos(List<Foto> lista, int identificadorUsuarioLogado)
        {
            /*if (lista.Where(d => !string.IsNullOrEmpty(d.CodigoFoto) && !d.Video.GetValueOrDefault()).Any())
            {
                ViagemBusiness bizViagem = new ViagemBusiness();
                Usuario itemUsuario = bizViagem.SelecionarUsuario(identificadorUsuarioLogado);
                bizViagem.AtualizarTokenUsuario(itemUsuario);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://photoslibrary.googleapis.com/v1/mediaItems:batchGet?" + string.Join("&", lista.Where(d => !string.IsNullOrEmpty(d.CodigoFoto) && !d.Video.GetValueOrDefault()).Select(d => "mediaItemIds=" + d.CodigoFoto).ToArray()));
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + itemUsuario.Token);


                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader responseReader = new StreamReader(response.GetResponseStream());

                    string responseStr = responseReader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic resultado = JObject.Parse(responseStr);
                        Newtonsoft.Json.Linq.JArray obj = resultado.mediaItemResults;
                        foreach (dynamic itemObj in obj.ToArray())
                        {
                            string Id = itemObj.mediaItem.id;
                            string BaseUrl = itemObj.mediaItem.baseUrl;
                            var itemFoto = lista.Where(d => d.CodigoFoto == Id).FirstOrDefault();
                            itemFoto.LinkFoto = BaseUrl;
                            itemFoto.LinkThumbnail = BaseUrl + "=w256-h256-c";

                        }
                    }
                }
                catch (WebException ex)
                {
                    StreamReader responseReader = new StreamReader(ex.Response.GetResponseStream());
                    string responseStr = responseReader.ReadToEnd();


                }
            }*/
        }



        public List<PontoMapa> ListarPontosViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte, string Tipo, int IdentificadorUsuarioLogado)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                var lista = data.ListarPontosViagem(IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte, Tipo,IdentificadorUsuarioLogado);

               
                

                return lista;
            }
        }

        public List<LinhaMapa> ListarLinhasViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte, string Tipo)
        {
            List<LinhaMapa> lista = new List<LinhaMapa>();
            if (String.IsNullOrEmpty(Tipo) || Tipo == "D")
            {
                using (ConsultaRepository data = new ConsultaRepository())
                {
                    ResumoViagem itemResumo = new ResumoViagem();
                    var ListaIntervaloDeslocamento = data.CarregarResumoViagemAerea(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                    var ListaIntervaloAtracao = data.CarregarResumoAtracao(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);

                    var ListaPosicoes = data.ListarPosicao(IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                    Posicao itemPosicaoAnterior = null;
                    LinhaMapa itemLinha = new LinhaMapa();
                    itemLinha.Tipo = "N";
                    itemLinha.Pontos = new List<PontoMapa>();
                    lista.Add(itemLinha);
                    foreach (var itemPosicao in ListaPosicoes.OrderBy(d => d.DataGMT))
                    {
                        if (itemPosicaoAnterior != null)
                        {

                            if (itemLinha.Tipo != "D" && itemPosicao.DataLocal.GetValueOrDefault().Subtract(itemPosicaoAnterior.DataLocal.GetValueOrDefault()).TotalMinutes > 90)
                            {
                                var tipo = itemLinha.Tipo;
                                itemLinha = new LinhaMapa();
                                itemLinha.Tipo = tipo;
                                itemLinha.Pontos = new List<PontoMapa>();
                                lista.Add(itemLinha);
                            }

                            if (!ListaIntervaloDeslocamento.Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any() &&
                                !ListaIntervaloAtracao.Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any())
                            {
                                if (itemLinha.Tipo != "N")
                                {
                                    itemLinha.Pontos.Add(new PontoMapa() { Latitude = itemPosicao.Latitude, Longitude = itemPosicao.Longitude });
                                    itemLinha = new LinhaMapa();
                                    itemLinha.Tipo = "N";
                                    itemLinha.Pontos = new List<PontoMapa>();
                                    lista.Add(itemLinha);
                                }
                            }

                            else if (ListaIntervaloDeslocamento.Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any())
                            {

                                if (itemLinha.Tipo != "D")
                                {
                                    itemLinha.Pontos.Add(new PontoMapa() { Latitude = itemPosicao.Latitude, Longitude = itemPosicao.Longitude });

                                    itemLinha = new LinhaMapa();
                                    itemLinha.Tipo = "D";
                                    itemLinha.Pontos = new List<PontoMapa>();
                                    lista.Add(itemLinha);
                                }
                            }
                            else if (ListaIntervaloAtracao.Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any())
                            {

                                if (itemLinha.Tipo != "A")
                                {
                                    itemLinha.Pontos.Add(new PontoMapa() { Latitude = itemPosicao.Latitude, Longitude = itemPosicao.Longitude });

                                    itemLinha = new LinhaMapa();
                                    itemLinha.Tipo = "A";
                                    itemLinha.Pontos = new List<PontoMapa>();
                                    lista.Add(itemLinha);
                                }
                            }
                        };

                        itemLinha.Pontos.Add(new PontoMapa() { Latitude = itemPosicao.Latitude, Longitude = itemPosicao.Longitude });

                        itemPosicaoAnterior = itemPosicao;

                    }

                }
            }
            return lista.Where(d => d.Pontos.Count() > 1).ToList();
        }

        public List<CalendarioRealizado> CarregarCalendarioRealizado(int? IdentificadorViagem, int? IdentificadorUsuario)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.CarregarCalendarioRealizado(IdentificadorViagem, IdentificadorUsuario);
            }
        }

        public ResumoViagem CarregarResumoViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            ResumoViagem itemResumo = new ResumoViagem();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                var ListaIntervaloAtracao = data.CarregarResumoAtracao(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                data.CarregarResumoHotel(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                data.CarregarResumoRefeicao(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                data.CarregarResumoCompra(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                data.CarregarResumoDiversos(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                var ListaIntervaloDeslocamento = data.CarregarResumoViagemAerea(itemResumo, IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                var ListaPosicoes = data.ListarPosicao(IdentificadorViagem, IdentificadorUsuario, DataDe, DataAte);
                itemResumo.KmDeslocamento = 0;
                itemResumo.KmTotaisDeslocados = 0;
                itemResumo.KmCaminhados = 0;
                Posicao itemPosicaoAnterior = null;


                foreach (var itemPosicao in ListaPosicoes.OrderBy(d => d.DataGMT))
                {
                    if (itemPosicaoAnterior != null)
                    {
                        int DistanciaPontos = Convert.ToInt32(CalcularDistanciaPontos(itemPosicao, itemPosicaoAnterior));
                        itemResumo.KmTotaisDeslocados += DistanciaPontos;
                        if (!ListaIntervaloDeslocamento
                            .Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any()
                            &&
                            !ListaIntervaloAtracao
                            .Where(d => d.Partida <= itemPosicao.DataLocal && d.Chegada >= itemPosicao.DataLocal).Any())
                        {

                            itemResumo.KmCaminhados += DistanciaPontos;
                        }


                    }
                    itemPosicaoAnterior = itemPosicao;
                }
                itemResumo.KmTotaisDeslocados /= 1000;
                itemResumo.KmCaminhados /= 1000;
                itemResumo.KmAtracoes /= 1000;
                itemResumo.KmDeslocamento /= 1000;
                //foreach (var intervaloDeslocamento in ListaIntervaloDeslocamento)
                //{
                //    var PosicaoAnterior = ListaPosicoes.Where(d => d.DataLocal <= intervaloDeslocamento.Partida).OrderByDescending(d => d.DataLocal).FirstOrDefault();
                //    var PosicaoSeguinte = ListaPosicoes.Where(d => d.DataLocal >= intervaloDeslocamento.Chegada).OrderBy(d => d.DataLocal).FirstOrDefault();
                //    var ListaSelecionar = ListaPosicoes.ToList();
                //    if (PosicaoAnterior != null)
                //        ListaSelecionar = ListaSelecionar.Where(d => d.DataLocal >= PosicaoAnterior.DataLocal).ToList();
                //    if (PosicaoSeguinte != null)
                //        ListaSelecionar = ListaSelecionar.Where(d => d.DataLocal <= PosicaoSeguinte.DataLocal).ToList();
                //    Posicao itemAnterior = null;
                //    foreach (var itemPosicao in ListaSelecionar.OrderBy(d=>d.DataLocal))
                //    {
                //        if (itemAnterior != null)
                //            itemResumo.KmDeslocamento += Convert.ToInt32(CalcularDistanciaPontos(itemPosicao, itemAnterior));
                //        itemAnterior = itemPosicao;
                //    }

                //}
                itemResumo.KmDeslocamento /= 1000;


            }
            ViagemBusiness biz = new ViagemBusiness();
            itemResumo.CidadesRegistradas =
                biz.CarregarCidadeAtracao(IdentificadorViagem).Union(
                biz.CarregarCidadeComentario(IdentificadorViagem).Union(
            biz.CarregarCidadeHotel(IdentificadorViagem).Union(
             biz.CarregarCidadeLoja(IdentificadorViagem).Union(
            biz.CarregarCidadeRefeicao(IdentificadorViagem).Union(
             biz.CarregarCidadeSugestao(IdentificadorViagem).Union(
             biz.CarregarCidadeViagemAerea(IdentificadorViagem))))))).Select(d => d.Identificador).Distinct().Count();


            return itemResumo;
        }

        public double CalcularDistanciaPontos(Posicao p1, Posicao p2)
        {
            double pk = (double)(180d / Math.PI);

            double a1 = Convert.ToDouble(p1.Latitude.GetValueOrDefault()) / pk;
            double a2 = Convert.ToDouble(p1.Longitude.GetValueOrDefault()) / pk;
            double b1 = Convert.ToDouble(p2.Latitude.GetValueOrDefault()) / pk;
            double b2 = Convert.ToDouble(p2.Longitude.GetValueOrDefault()) / pk;

            double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
            double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
            double t3 = Math.Sin(a1) * Math.Sin(b1);

            double tt = Math.Acos(Math.Round(t1 + t2 + t3, 12));

            return 6366000 * tt;
        }

        public List<ConsultaRankings> ListarRankings(int? IdentificadorViagem, int? IdentificadorUsuario, bool ApenasAmigos, int? IdentificadorAmigo, string Tipo, int? NumeroRegistros, string CodigoGoogle, string Nome)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarRankings(IdentificadorViagem, IdentificadorUsuario, ApenasAmigos, IdentificadorAmigo, Tipo, NumeroRegistros, CodigoGoogle, Nome);
            }
        }

        public List<UsuarioConsulta> ListarAvaliacoesRankings(int? IdentificadorViagem, int? IdentificadorUsuario, bool ApenasAmigos, int? IdentificadorAmigo, string Tipo, string CodigoGoogle, string Nome)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarAvaliacoesRankings(IdentificadorViagem, IdentificadorUsuario, ApenasAmigos, IdentificadorAmigo, Tipo, CodigoGoogle, Nome);
            }
        }
    }
}
