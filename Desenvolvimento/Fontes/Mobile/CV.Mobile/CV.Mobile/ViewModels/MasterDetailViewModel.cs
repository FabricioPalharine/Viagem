﻿using CV.Mobile.Helpers;
using CV.Mobile.Interfaces;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Practices.ServiceLocation;
using MvvmHelpers;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Vibrate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class MasterDetailViewModel : BaseViewModel
    {

        private HubConnection cvHubConnection;
        private IHubProxy cvHubProxy;

        private MenuPage masterPage;
        private Page detailPage;
        private UsuarioLogado _ItemUsuario;
        private Viagem _ItemViagem;
        private IGeolocator locator;
        private Position ultimaPosicao = null;
        private Hotel _hotelAtual = null;
        private bool HotelDentro = false;
        private bool _ConectadoPrincipal = true;
        public MasterDetailViewModel(UsuarioLogado itemUsuario)
        {
            cvHubConnection = new HubConnection(String.Concat(Settings.BaseWebApi, "signalr/hubs"));

            locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            locator.AllowsBackgroundUpdates = true;
            locator.PositionChanged += Locator_PositionChanged;
            MenuViewModel vmMenu = new MenuViewModel(itemUsuario);
            _ItemUsuario = itemUsuario;
            vmMenu.ItemMenuSelecionado += async (itemMenu, novoStack) => { await SelecionarItemMenu(itemMenu, novoStack); };
            var paginaMenu = new MenuPage() { BindingContext = vmMenu };
            MasterPage = paginaMenu;
            var paginaDetalhe = new MenuInicialPage() { BindingContext = new MenuInicialViewModel() };
            DetailPage = paginaDetalhe;
            MessagingService.Current.Unsubscribe<Hotel>(MessageKeys.ManutencaoHotelSelecao);
            MessagingService.Current.Subscribe<Hotel>(MessageKeys.ManutencaoHotelSelecao, (service, item) =>
            {
                var hotel = _hotelAtual ?? new Hotel();
                if (item.Identificador == hotel.Identificador.GetValueOrDefault())
                {
                    if (item.DataSaidia.HasValue || item.DataExclusao.HasValue)
                        _hotelAtual = null;
                    else if (item.DataEntrada.HasValue)
                    {
                        _hotelAtual = item;
                        HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuario.Codigo).Where(d => !d.DataSaida.HasValue).Any();

                    }
                }
                else
                {
                    if (item.DataEntrada.HasValue && !item.DataExclusao.HasValue && !item.DataSaidia.HasValue)
                    {
                        _hotelAtual = item;
                        HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuario.Codigo).Where(d => !d.DataSaida.HasValue).Any();

                    }
                }
            });
            MessagingService.Current.Unsubscribe(MessageKeys.VerificarCalendario);
            MessagingService.Current.Subscribe(MessageKeys.VerificarCalendario, async (service) =>
            {
                if (ItemViagem != null)
                {
                    var Alerta = await DatabaseService.Database.ConsultarCalendarioAlerta();
                    if (Alerta != null)
                    {
                        CrossVibrate.Current.Vibration(1000);
                        MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
                        {
                            Title = "Informação",
                            Question = String.Format("Você agendou a visita a {0} as {1}", Alerta.Nome, Alerta.DataInicio.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm")),
                            Positive = "Avisar Novamento",
                            Negative = "Ignorar",
                            OnCompleted = new Action<bool>(async result =>
                            {
                                if (result)
                                {
                                    Alerta.DataProximoAviso = Alerta.DataProximoAviso.AddMinutes(5);
                                }
                                else
                                {
                                    Alerta.AvisarHorario = false;
                                    Alerta.DataAtualizacao = DateTime.Now.ToUniversalTime();
                                    if (ConectadoPrincipal)
                                    {
                                        using (ApiService srv = new ApiService())
                                        {
                                            var Resultado = await srv.SalvarCalendarioPrevisto(Alerta);
                                            if (Resultado.Sucesso)
                                            {
                                                Alerta.Identificador = Resultado.IdentificadorRegistro;
                                                await AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "CP", Alerta.Identificador.GetValueOrDefault(), false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Alerta.AtualizadoBanco = false;

                                    }

                                }
                                await DatabaseService.Database.SalvarCalendarioPrevisto(Alerta);

                            })
                        });
                    }
                }
            });
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            VerificarConectividade(false);
        }

        internal async Task VerificarConexaoSignalR(Viagem itemViagem)
        {
            if (CrossConnectivity.Current.IsConnected && Settings.AcompanhamentoOnline != "3")
            {
                if (Settings.AcompanhamentoOnline == "1")
                {
                    await ConectatSignalR(itemViagem);
                }
                else if (CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Wimax) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi))
                {
                    await ConectatSignalR(itemViagem);
                }
                else
                    PararHubSignalR();
            }
            else
                PararHubSignalR();

        }

        internal async void VerificarConectividade(bool VerificarSincronizacao)
        {
            bool Conectado = false;
            if (CrossConnectivity.Current.IsConnected && Settings.AcompanhamentoOnline != "3")
            {
                if (Settings.AcompanhamentoOnline == "1")
                {
                    Conectado = true;
                }
                else if (CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Wimax) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi))
                {
                    Conectado = true;
                }
            }
            if (!Conectado)
            {
                var itemControle = await DatabaseService.Database.GetControleSincronizacaoAsync();
                if (itemControle.SincronizadoEnvio)
                {
                    itemControle.SincronizadoEnvio = false;
                    await DatabaseService.Database.SalvarControleSincronizacao(itemControle);
                }
            }
            bool ConectadoAnterior = _ConectadoPrincipal;
            _ConectadoPrincipal = Conectado;
            if (VerificarSincronizacao)
            {
                IniciarProcessoSincronizacao(ConectadoAnterior);
            }
        }

        private async void IniciarProcessoSincronizacao(bool ConectadoAnterior)
        {
            if (ItemViagem.Edicao)
            {
                try
                {
                    if (!ConectadoAnterior)
                        await VerificarSincronizacaoDados();
                    VerificarEnvioFotos();
                    VerificarEnvioVideos();
                }
                catch
                {

                }
            }
        }

        public async Task VerificarSincronizacaoDados()
        {
            if (CrossConnectivity.Current.IsConnected && Settings.ModoSincronizacao != "3" &&
         (Settings.ModoSincronizacao == "1" || CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop) ||
                 CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Wimax) ||
                 CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi)))
            {
                await SincronizarDados(false);
            }
        }

        public async void VerificarEnvioVideos()
        {
            if (CrossConnectivity.Current.IsConnected && Settings.ModoVideo != "3" &&
          (Settings.ModoVideo == "1" || CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop) ||
                  CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Wimax) ||
                  CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi)))
            {
                await EnviarVideos();
            }
        }

        public async Task EnviarVideos()
        {

            var ListaFotos = await DatabaseService.Database.ListarUploadFoto_Video(true);
            foreach (var itemFoto in ListaFotos.Where(d => d.IdentificadorAtracao.GetValueOrDefault(0) >= 0 && d.IdentificadorHotel.GetValueOrDefault(0) >= 0 && d.IdentificadorRefeicao.GetValueOrDefault(0) >= 0))
            {
                var DadosFoto = ServiceLocator.Current.GetInstance<IFileHelper>().CarregarStreamFile(itemFoto.CaminhoLocal);
                using (ApiService srv = new ApiService())
                {

                    var ItemUsuario = await srv.CarregarUsuario(_ItemUsuario.Codigo);
                    if (ItemUsuario.DataToken.GetValueOrDefault().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now.ToUniversalTime())
                    {
                        using (AccountsService srvAccount = new AccountsService())
                        {
                            await srvAccount.AtualizarTokenUsuario(ItemUsuario);
                        }
                    }

                    new ConfiguracaoViewModel().GravarVideoYouTube(itemFoto, DadosFoto, ItemUsuario, false);


                }
                await DatabaseService.Database.ExcluirUploadFoto(itemFoto);
            }

        }

        public async void VerificarEnvioFotos()
        {
            if (CrossConnectivity.Current.IsConnected && Settings.ModoImagem != "3" &&
            (Settings.ModoImagem == "1" || CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Wimax) ||
                    CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi)))
            {
                await EnviarFotos();
            }
        }

        public async Task EnviarFotos()
        {
            var ListaFotos = await DatabaseService.Database.ListarUploadFoto_Video(false);
            foreach (var itemFoto in ListaFotos.Where(d => d.IdentificadorAtracao.GetValueOrDefault(0) >= 0 && d.IdentificadorHotel.GetValueOrDefault(0) >= 0 && d.IdentificadorRefeicao.GetValueOrDefault(0) >= 0))
            {
                byte[] DadosFoto = ServiceLocator.Current.GetInstance<IFileHelper>().CarregarDadosFile(itemFoto.CaminhoLocal);
                using (ApiService srv = new ApiService())
                {

                    var ItemUsuario = await srv.CarregarUsuario(_ItemUsuario.Codigo);
                    if (ItemUsuario.DataToken.GetValueOrDefault().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now.ToUniversalTime())
                    {
                        using (AccountsService srvAccount = new AccountsService())
                        {
                            await srvAccount.AtualizarTokenUsuario(ItemUsuario);
                        }
                    }
                    using (PhotosService srvFoto = new PhotosService(ItemUsuario.Token))
                    {
                        await srvFoto.SubirFoto(_ItemViagem.CodigoAlbum, DadosFoto, itemFoto);
                    }
                    var resultadoAPI = await srv.SubirImagem(itemFoto);
                    await AtualizarViagem(_ItemViagem.Identificador.GetValueOrDefault(), "F", resultadoAPI.IdentificadorRegistro.GetValueOrDefault(), true);
                }
                await DatabaseService.Database.ExcluirUploadFoto(itemFoto);
            }
        }

        private async Task ConectatSignalR(Viagem itemViagem)
        {

            await IniciarHubSignalR();

            if (itemViagem != null)
                await ConectarViagem(itemViagem.Identificador.GetValueOrDefault(), itemViagem.Edicao);
        }

        private async void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            await VerificarConexaoSignalR(ItemViagem);
            VerificarConectividade(true);
        }

        private async void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            if (e.Position.Speed > 0)
            {
                if (_ItemViagem != null)
                {
                    Posicao itemPosicao = new Posicao()
                    {
                        DataGMT = e.Position.Timestamp.DateTime,
                        DataLocal = new DateTime(DateTime.Now.Ticks),
                        IdentificadorUsuario = _ItemUsuario.Codigo,
                        IdentificadorViagem = _ItemViagem.Identificador,
                        Latitude = e.Position.Latitude,
                        Longitude = e.Position.Longitude,
                        Velocidade = e.Position.Speed

                    };
                    ultimaPosicao = e.Position;

                    if (ConectadoPrincipal)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            try
                            {
                                await srv.SalvarPosicao(itemPosicao);
                            }
                            catch
                            {
                                await DatabaseService.Database.SalvarPosicao(itemPosicao);
                            }
                        }
                    }
                    else
                        await DatabaseService.Database.SalvarPosicao(itemPosicao);
                }
                if (_hotelAtual != null && _hotelAtual.Raio > 0 && _hotelAtual.Latitude.HasValue && _hotelAtual.Longitude.HasValue && e.Position.Speed > 0)
                {
                    var DistanciaAtual = GetDistanceTo(new Position() { Longitude = _hotelAtual.Longitude.Value, Latitude = _hotelAtual.Latitude.Value }, e.Position);
                    bool estaDentro = DistanciaAtual.Meters <= _hotelAtual.Raio;
                    if (estaDentro != HotelDentro)
                    {
                        HotelDentro = estaDentro;
                        HotelEvento itemEvento = new HotelEvento() { DataEntrada = DateTime.Now, IdentificadorHotel = _hotelAtual.Identificador, IdentificadorUsuario = ItemUsuario.Codigo, DataAtualizacao = DateTime.Now.ToUniversalTime() };
                        if (!estaDentro)
                            itemEvento.DataSaida = DateTime.Now;
                        if (ConectadoPrincipal)
                        {
                            using (ApiService srv = new ApiService())
                            {
                                var Resultado = await srv.SalvarHotelEvento(itemEvento);
                                if (Resultado.Sucesso)
                                {
                                    var Jresultado = (JObject)Resultado.ItemRegistro;
                                    var pItemEvento = Jresultado.ToObject<HotelEvento>();
                                    var itemBanco = await DatabaseService.Database.RetornarHotelEvento(pItemEvento.Identificador);
                                    if (itemBanco != null)
                                        pItemEvento.Id = itemBanco.Id;
                                    await DatabaseService.Database.SalvarHotelEvento(pItemEvento);
                                    await AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "HE", pItemEvento.Identificador.GetValueOrDefault(), false);

                                }
                            }
                        }
                        else
                        {
                            var itemAtual = await DatabaseService.Database.RetornarUltimoHotelEvento_IdentificadorHotel_IdentificadorUsuario(_hotelAtual.Identificador, ItemUsuario.Codigo);
                            if (itemAtual != null && !estaDentro)
                            {
                                itemAtual.DataAtualizacao = DateTime.Now.ToUniversalTime();
                                itemAtual.AtualizadoBanco = false;
                                itemAtual.DataSaida = DateTime.Now;
                                await DatabaseService.Database.SalvarHotelEvento(itemAtual);
                            }
                            else if (itemAtual == null && estaDentro)
                            {
                                itemEvento.AtualizadoBanco = false;
                                await DatabaseService.Database.SalvarHotelEvento(itemEvento);
                            }

                        }

                    }
                }
            }
        }

        public MenuPage MasterPage
        {
            get
            {
                return masterPage;
            }

            set
            {
                SetProperty(ref masterPage, value);
            }
        }

        public Page DetailPage
        {
            get
            {
                return detailPage;
            }

            set
            {
                SetProperty(ref detailPage, value);
            }
        }

        public bool IsPresented
        {
            get
            {
                return isPresented;
            }

            set
            {
                SetProperty(ref isPresented, value);
            }
        }

        public UsuarioLogado ItemUsuario
        {
            get
            {
                return _ItemUsuario;
            }

            set
            {
                SetProperty(ref _ItemUsuario, value);
            }
        }

        public Viagem ItemViagem
        {
            get
            {
                return _ItemViagem;
            }

            set
            {
                SetProperty(ref _ItemViagem, value);
                CarregarHotelAtual();
                (masterPage.BindingContext as MenuViewModel).ItemViagem = value;
                (detailPage.BindingContext as MenuInicialViewModel).ItemViagem = value;
            }
        }

        public bool ConectadoPrincipal
        {
            get
            {
                return _ConectadoPrincipal;
            }

            set
            {
                _ConectadoPrincipal = value;
            }
        }

        private async void CarregarHotelAtual()
        {
            if (ConectadoPrincipal)
            {
                using (ApiService srv = new ApiService())
                {
                    var Hoteis = await srv.ListarHotel(new CriterioBusca() { Situacao = 1 });
                    if (Hoteis.Any())
                    {
                        _hotelAtual = await srv.CarregarHotel(Hoteis.OrderByDescending(d => d.DataEntrada).Select(d => d.Identificador).FirstOrDefault());
                        HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuario.Codigo).Where(d => !d.DataSaida.HasValue).Any();
                    }
                }
            }
            else
            {
                var Hoteis = await DatabaseService.Database.ListarHotel(new CriterioBusca() { Situacao = 1 });
                if (Hoteis.Any())
                {
                    _hotelAtual = await DatabaseService.CarregarHotel(Hoteis.OrderByDescending(d => d.DataEntrada).Select(d => d.Identificador).FirstOrDefault());
                    HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuario.Codigo).Where(d => !d.DataSaida.HasValue).Any();

                }
            }
        }

        public Xamarin.Forms.Maps.Distance GetDistanceTo(Position posicao, Position other)
        {

            double pk = (double)(180d / Math.PI);

            double a1 = posicao.Latitude / pk;
            double a2 = posicao.Longitude / pk;
            double b1 = other.Latitude / pk;
            double b2 = other.Longitude / pk;

            double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
            double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
            double t3 = Math.Sin(a1) * Math.Sin(b1);
            double tt = Math.Acos(t1 + t2 + t3);

            return Xamarin.Forms.Maps.Distance.FromMeters(6366000 * tt);

            //var d1 = posicao.Latitude * (Math.PI / 180.0);
            //var num1 = posicao.Longitude * (Math.PI / 180.0);
            //var d2 = other.Latitude * (Math.PI / 180.0);
            //var num2 = other.Longitude * (Math.PI / 180.0) - num1;
            //var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
            //         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            //var meters = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            //return Xamarin.Forms.Maps.Distance.FromMeters(meters);
        }

        public async Task IniciarControlePosicao()
        {
            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                if (_ItemViagem != null && _ItemViagem.Edicao && _ItemViagem.Aberto && _ItemViagem.DataInicio < DateTime.Now && _ItemViagem.ControlaPosicaoGPS)
                {
                    if (!locator.IsListening)
                        await locator.StartListeningAsync(60, 5, true);
                }
                else
                {
                    if (locator.IsListening)
                        await locator.StopListeningAsync();
                }
            }
        }

        public async Task<Position> RetornarPosicaoGPS()
        {
            if (ultimaPosicao == null)
            {
                try
                {
                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 30000);
                    return position;
                }
                catch
                {
                    return null;
                }
            }
            else
                return ultimaPosicao;
        }

        private bool isPresented;

        private async Task SelecionarItemMenu(Page itemMenu, bool NovoStack)
        {
            (Application.Current.MainPage as Xamarin.Forms.MasterDetailPage).IsPresented = false;

            if (NovoStack)
            {
                (Application.Current.MainPage as Xamarin.Forms.MasterDetailPage).Detail = new NavigationPage(itemMenu);
            }
            else
            {
                var _Navigation = ((Views.MasterDetailPage)Application.Current?.MainPage).Detail?.Navigation;
                var task = _Navigation?.PushAsync(itemMenu);
                if (task != null)
                    await task;
            }

        }

        public void PreencherPaginasViagem(Viagem itemViagem)
        {
            var _Navigation = ((Views.MasterDetailPage)Application.Current?.MainPage).Detail?.Navigation;
            foreach (var Pagina in _Navigation.NavigationStack)
            {
                if (Pagina.BindingContext is MenuInicialViewModel)
                {
                    ((MenuInicialViewModel)Pagina.BindingContext).ViagemSelecionada = true;
                    ((MenuInicialViewModel)Pagina.BindingContext).ItemViagem = itemViagem;
                }
            }
        }

        public async Task IniciarHubSignalR()
        {
            if (cvHubConnection.State != ConnectionState.Connected)
            {
                cvHubProxy = cvHubConnection.CreateHubProxy("Viagem");
                cvHubProxy.On<string, int, bool>("avisarAlertaAtualizacao", (Tipo, Identificador, Inclusao) =>
               {
                   VerificarAcaoAlteracao(Tipo, Identificador);
               });

                cvHubProxy.On<AlertaUsuario>("enviarAlertaRequisicao", (itemAlerta) =>
                {
                });
                await cvHubConnection.Start();
                await ConectarUsuario(ItemUsuario.Codigo);
            }
        }

        private async void VerificarAcaoAlteracao(string tipo, int identificador)
        {
            if (ItemViagem.Edicao)
            {
                await DatabaseService.AtualizarBancoRecepcaoAcao(tipo, identificador, ItemUsuario);
            }
            MessagingService.Current.SendMessage<AtualizacaoConsulta>(MessageKeys.AtualizarConsulta, new AtualizacaoConsulta() { Identificador = identificador, Tipo = tipo }); ;
        }

        public async Task ConectarUsuario(int IdentificadorUsuario)
        {
            if (cvHubConnection.State == ConnectionState.Connected)
                await cvHubProxy.Invoke("conectarUsuario", IdentificadorUsuario);
        }
        public async Task DesconectarUsuario(int IdentificadorUsuario)
        {
            if (cvHubConnection.State == ConnectionState.Connected)

                await cvHubProxy.Invoke("desconectarUsuario", IdentificadorUsuario);
        }

        public async Task RequisitarAmizade(int IdentificadorUsuario, int IdentificadorRequisicao)
        {
            if (cvHubConnection.State == ConnectionState.Connected)

                await cvHubProxy.Invoke("requisitarAmizade", IdentificadorUsuario, IdentificadorRequisicao);
        }

        public async Task ConectarViagem(int IdentificadorUsuario, bool Edicao)
        {
            if (cvHubConnection.State == ConnectionState.Connected)

                await cvHubProxy.Invoke("ConectarViagem", IdentificadorUsuario, Edicao);
        }
        public async Task SugerirVisitaViagem(Sugestao itemSugestao)
        {
            if (cvHubConnection.State == ConnectionState.Connected)

                await cvHubProxy.Invoke("sugerirVisitaViagem", itemSugestao);
        }

        public async Task AtualizarViagem(int IdentificadorViagem, string TipoAtualizacao, int Identificador, bool Inclusao)
        {
            if (cvHubConnection.State == ConnectionState.Connected)
            {
                var itemControle = await DatabaseService.Database.GetControleSincronizacaoAsync();
                if (itemControle.SincronizadoEnvio)
                {
                    itemControle.UltimaDataRecepcao = DateTime.Now.ToUniversalTime();
                    await DatabaseService.Database.SalvarControleSincronizacao(itemControle);
                }
                await cvHubProxy.Invoke("viagemAtualizada", IdentificadorViagem, TipoAtualizacao, Identificador, Inclusao);
            }
        }


        public void PararHubSignalR()
        {
            if (cvHubConnection.State == ConnectionState.Connected)

                cvHubConnection.Stop();
        }

        public async Task SincronizarDados(bool exibeAlerta)
        {
            using (ApiService srv = new ApiService())
            {
                if (CrossConnectivity.Current.IsConnected && await srv.VerificarOnLine())
                {

                    var itemCS = await DatabaseService.Database.GetControleSincronizacaoAsync();
                    var DataSincronizacao = DateTime.Now.ToUniversalTime();
                    var DadosSincronizar = await srv.RetornarAtualizacoes(new CriterioBusca() { DataInicioDe = itemCS.UltimaDataRecepcao });

                    await DatabaseService.SincronizarDadosServidorLocal(itemCS, DadosSincronizar, ItemUsuario, DataSincronizacao);

                    var item = await DatabaseService.CarregarDadosEnvioSincronizar();
                    var resultadoSincronizacao = await srv.SincronizarDados(item);
                    itemCS.UltimaDataEnvio = DateTime.Now.ToUniversalTime();
                    await DatabaseService.AjustarDePara(item, resultadoSincronizacao, itemCS,this);


                }
                else
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Conexão Off-Line",
                        Message = "Essa funcionalidade necessita que a conexão esteja com acesso a internet",
                        Cancel = "OK"
                    });
                }
            }
        }

    }
}
