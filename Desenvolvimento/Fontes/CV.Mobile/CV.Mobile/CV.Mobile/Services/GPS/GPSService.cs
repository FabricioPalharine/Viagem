using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.PlatformSpecifcs;
using CV.Mobile.Services.Settings;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Services.GPS
{
    public class GPSService: IGPSService
    {
        private Position posicaoAtual = null;
        private ISettingsService _settingsService;
        private IApiService _apiService;
        private IDatabase _database;
        private bool GPSAtivo = false;
        private readonly ILocationConsent _locationConsent;
        public GPSService (ISettingsService settingsService, IApiService apiService, IDatabase database)
        {
            CrossGeolocator.Current.DesiredAccuracy = 2;
            _locationConsent = DependencyService.Get<ILocationConsent>();
            _settingsService = settingsService;
            _apiService = apiService;
            _database = database;
           
        }

        public async Task IniciarGPS()
        {
            if (CrossGeolocator.IsSupported && CrossGeolocator.Current.IsGeolocationEnabled && !GPSAtivo)
            {
                GPSAtivo = true;
                await _locationConsent.GetLocationConsent();
                Device.StartTimer(TimeSpan.FromSeconds(10), () =>
                {
                    AtualizarPosicao();

                    return GPSAtivo;

                });
            }
           // return Task.FromResult(true);
        }

        public Task PararGPS()
        {
            if ( GPSAtivo)
            {
                GPSAtivo = false;
            }
            return Task.FromResult(true);
        }
        private bool atualizando = false;
        private async void PositionChanged(object sender, PositionEventArgs e)
        {
            if (!atualizando)
            {
                atualizando = true;
                await AtualizarPosicao(e.Position);
            }
        }

        private async void AtualizarPosicao()
        {
            if (!atualizando)
            {
                atualizando = true;
                try
                {
                    var posicao = await CrossGeolocator.Current.GetLastKnownLocationAsync();
                    if (posicao != null)
                    await AtualizarPosicao(posicao);
                }
                finally
                {
                    atualizando = false;
                }
            }
        }

        private async Task AtualizarPosicao(Position e)
        {
            
               
                try
                {
                    bool sincronizacaoAtiva = Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline);
                    bool atualizarPosicao = true;
                    if (posicaoAtual != null)
                    {
                        var ultimaPosicao = new Position(posicaoAtual);
                        await CalcularEventoHotel(e, ultimaPosicao, sincronizacaoAtiva);
                        var distancia = Convert.ToDecimal(Xamarin.Essentials.Location.CalculateDistance(posicaoAtual.Latitude, posicaoAtual.Longitude, e.Latitude, e.Longitude, Xamarin.Essentials.DistanceUnits.Kilometers)) * 1000;

                        if (distancia < 2)
                        {
                            atualizarPosicao = false;
                        }
                        else
                        {
                            await CalcularDistanciaAtracao(e, ultimaPosicao);
                            await CalcularDistanciaDeslocamento(e, ultimaPosicao);
                        }


                    }

                    if (atualizarPosicao)
                    {
                        Posicao posicao = new Posicao()
                        {
                            DataGMT = DateTime.UtcNow,
                            DataLocal = DateTime.SpecifyKind(e.Timestamp.Date, DateTimeKind.Unspecified),
                            IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo,
                            IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
                            Velocidade = e.Speed,
                            Latitude = e.Latitude,
                            Longitude = e.Longitude
                        };
                        if (sincronizacaoAtiva)
                            await _apiService.SalvarPosicao(posicao);
                        else
                            await _database.SalvarPosicao(posicao);
                        posicaoAtual = e;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    atualizando = false;
                }
            
        }

        private async Task CalcularEventoHotel(Position e, Position ultimaPosicao, bool sincronizacaoAtiva)
        {
            var hotelAtual = await _database.ListarHotelAtual();
            if (hotelAtual != null && hotelAtual.Latitude.HasValue && hotelAtual.Longitude.HasValue && hotelAtual.Raio.GetValueOrDefault(0) > 0)
            {
                var eventoAtual = hotelAtual.Eventos.Where(d => !d.DataSaida.HasValue).FirstOrDefault();
                if(eventoAtual == null || !hotelAtual.Movel)
                {
                    var distancia = Xamarin.Essentials.Location.CalculateDistance(hotelAtual.Latitude.Value, hotelAtual.Longitude.Value, e.Latitude, e.Longitude, Xamarin.Essentials.DistanceUnits.Kilometers);
                    distancia = distancia * 1000;
                    if (eventoAtual == null)
                    {
                        if (distancia < hotelAtual.Raio.Value)
                        {
                            var ultimoEvento = hotelAtual.Eventos.FirstOrDefault();
                            if (ultimoEvento != null && DateTime.Now.AddMinutes(-2) < ultimoEvento.DataSaida.Value )
                            {
                                ultimoEvento.LatitudeSaida = null;
                                ultimoEvento.LongitudeSaida = null;
                                ultimoEvento.DataSaida = null;
                                ultimoEvento.HoraSaida = null;
                                await SalvarEvento(ultimoEvento, sincronizacaoAtiva);
                            }
                            else
                            {
                                HotelEvento novoEvento = new HotelEvento() { DataAtualizacao = DateTime.UtcNow, IdentificadorHotel = hotelAtual.Identificador, LatitudeEntrada = e.Latitude, LongitudeEntrada = e.Longitude, IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, HoraEntrada = DateTime.Now.TimeOfDay, DataEntrada = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified) };
                                await SalvarEvento(novoEvento, sincronizacaoAtiva);
                            }
                        }
                    }
                    else if (distancia > hotelAtual.Raio)
                    {
                        if (DateTime.Now.AddMinutes(-2) < eventoAtual.DataEntrada.Value)
                        {
                            eventoAtual.DataAtualizacao = DateTime.UtcNow;
                            eventoAtual.DataExclusao = DateTime.UtcNow;
                            await SalvarEvento(eventoAtual, sincronizacaoAtiva);
                        }
                        else
                        {
                            eventoAtual.DataAtualizacao = DateTime.UtcNow;
                            eventoAtual.DataSaida = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                            eventoAtual.HoraSaida = DateTime.Now.TimeOfDay;
                            eventoAtual.LatitudeSaida = e.Latitude;
                            eventoAtual.LongitudeSaida = e.Longitude;
                            await SalvarEvento(eventoAtual, sincronizacaoAtiva);

                        }

                    }

                }
            }
        }

        private async Task SalvarEvento(HotelEvento ultimoEvento, bool sincronizacaoAtiva)
        {
            if (!sincronizacaoAtiva)
            {
                var resultado = await _apiService.SalvarHotelEvento(ultimoEvento);
                if (resultado != null)
                {
                    ultimoEvento.Identificador = ultimoEvento.Identificador.HasValue ? ultimoEvento.Identificador : resultado.IdentificadorRegistro;
                    ultimoEvento.AtualizadoBanco = true;
                    await _database.SalvarHotelEvento(ultimoEvento);

                }
            }
            else
            {
                ultimoEvento.AtualizadoBanco = false;
                await _database.SalvarHotelEvento(ultimoEvento);

            }
            MessagingCenter.Instance.Send(this, MessageKeys.AjustarEventoHotel, ultimoEvento);
        }

        private async Task CalcularDistanciaAtracao(Position e, Position ultimaPosicao)
        {
            var atracoesAberta = await _database.ListarAtracaoAberta();
            foreach (var atracao in atracoesAberta)
            {
                var posicaoComparar = ultimaPosicao;
                if (atracao.Chegada > ultimaPosicao.Timestamp && atracao.Latitude.HasValue && atracao.Longitude.HasValue)
                {
                    posicaoComparar = new Position(atracao.Latitude.Value, atracao.Longitude.Value);
                }
                atracao.Distancia = atracao.Distancia.GetValueOrDefault(0) + Convert.ToDecimal(Xamarin.Essentials.Location.CalculateDistance(posicaoComparar.Latitude, posicaoComparar.Longitude, e.Latitude, e.Longitude, Xamarin.Essentials.DistanceUnits.Kilometers));
                await _database.SalvarAtracao(atracao);
                MessagingCenter.Send(this, MessageKeys.AjustarDistanciaAtracao,atracao);
            }
        }

        private async Task CalcularDistanciaDeslocamento(Position e, Position ultimaPosicao)
        {
            var deslocamentosAberto = await _database.ListarViagemAereaAberto();
            foreach (var deslocamento in deslocamentosAberto)
            {
                var posicaoComparar = ultimaPosicao;
                
                deslocamento.Distancia = deslocamento.Distancia.GetValueOrDefault(0) + Convert.ToDecimal(Xamarin.Essentials.Location.CalculateDistance(posicaoComparar.Latitude, posicaoComparar.Longitude, e.Latitude, e.Longitude, Xamarin.Essentials.DistanceUnits.Kilometers));
                await _database.SalvarViagemAerea(deslocamento);
                MessagingCenter.Send(this, MessageKeys.AjustarDistanciaDeslocamento, deslocamento);
            }
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
        }


        public async Task<PosicaoMapa> RetornarPosicao()
        {
            PosicaoMapa posicao = null;
            try
            {
                Position location = null;
                if (posicaoAtual != null && DateTime.Now.Subtract(posicaoAtual.Timestamp.Date).TotalSeconds < 30)
                {
                    location = posicaoAtual;

                }
                else
                {
                    posicaoAtual = location = await CrossGeolocator.Current.GetLastKnownLocationAsync() ;
                    if (location == null)
                        posicaoAtual = location = await CrossGeolocator.Current.GetPositionAsync();
                }
                 posicao = new PosicaoMapa() { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            catch
            {

            }
            return posicao;
        }
        
    }
}
