using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Fotos;
using CV.Mobile.Services.GoogleToken;
using CV.Mobile.Services.GPS;
using CV.Mobile.Services.Settings;
using CV.Mobile.Services.Sincronizacao;
using CV.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile
{
    public class AppShellViewModel: BaseViewModel
    {
        private bool _ViagemSelecionada = false;
        private UsuarioLogado _itemUsuario = null;
        private bool _EdicaoViagem = false;
        private bool _VisualizarGastos = false;
        private bool? _GPSIniciado = null;
        private bool? _ViagemAberta = null;
        private bool _EdicaoDadoViagem = false;
        private bool _ViagemCarregada = false;
        private bool? _exibirGPS = null;
        private bool? _editarViagem = null;

        private readonly IDataService _dataService;
        private readonly IApiService _apiService;
        private readonly IDatabase _database;
        private readonly IGPSService _gps;
        private readonly ISincronizacao _sincronizacao;
        private readonly ISettingsService _settingsService;
        private readonly IFoto _foto;
        private readonly IAccountSevice _accountSevice;

        public AppShellViewModel(IDataService dataService, IApiService apiService, IDatabase database, IGPSService gPSService, ISincronizacao sincronizacao,
            ISettingsService settingsService, IFoto foto, IAccountSevice accountSevice)
        {
            _dataService = dataService;
            _apiService = apiService;
            _database = database;
            _gps = gPSService;
            _sincronizacao = sincronizacao;
            _settingsService = settingsService;
            _accountSevice = accountSevice;
            _foto = foto;
            MessagingCenter.Unsubscribe<ViewModels.Viagens.ViagemListaViewModel, Viagem>(this, MessageKeys.SelecionarViagem);
            MessagingCenter.Unsubscribe<ViewModels.Viagens.ViagemEdicaoViewModel, Viagem>(this, MessageKeys.SelecionarViagem);
            MessagingCenter.Subscribe<ViewModels.Viagens.ViagemListaViewModel, Viagem>(this, MessageKeys.SelecionarViagem, async (sender, arg) =>
            {
                await TrocarViagem(arg);
            });
            MessagingCenter.Subscribe<ViewModels.Viagens.ViagemCriacaoViewModel, Viagem>(this, MessageKeys.SelecionarViagem, async (sender, arg) =>
            {
                await TrocarViagem(arg);
            });
            MessagingCenter.Subscribe<ViewModels.Viagens.ViagemEdicaoViewModel, Viagem>(this, MessageKeys.SelecionarViagem, async (sender, arg) =>
            {
                await TrocarViagem(arg);
            });

        }

        private async Task TrocarViagem(Viagem viagem)
        {
            ViagemCarregada = false;
            await _gps.PararGPS();
            await _database.LimparBancoViagem();
            Viagem itemViagem = await _apiService.CarregarViagem(viagem.Identificador);
            if (itemViagem.Identificador.HasValue)
            {
                var DadosViagem = await _apiService.SelecionarViagem(itemViagem.Identificador);
                itemViagem.VejoGastos = DadosViagem.VerCustos;
                itemViagem.Edicao = DadosViagem.PermiteEdicao;
                itemViagem.Aberto = DadosViagem.Aberto;
                if (itemViagem.Edicao && itemViagem.Aberto)
                {
                    ControleSincronizacao itemCS = new ControleSincronizacao();
                    itemCS.IdentificadorViagem = itemViagem.Identificador;
                    itemCS.SincronizadoEnvio = false;
                    itemCS.UltimaDataEnvio = DateTime.Now.ToUniversalTime();
                    itemCS.UltimaDataRecepcao = new DateTime(1900, 01, 01);
                    await _database.SalvarControleSincronizacao(itemCS);
                    await _database.SalvarViagemAsync(itemViagem);
                    _dataService.SincronizarParticipanteViagem(itemViagem);
                    //ConectarViagem(itemViagem.Identificador.GetValueOrDefault(), itemViagem.Edicao);
                    try
                    {
                        var DadosSincronizar = await _apiService.RetornarAtualizacoes(new CriterioBusca() { DataInicioDe = itemCS.UltimaDataRecepcao });

                        await _dataService.SincronizarDadosServidorLocal(itemCS, DadosSincronizar, DadosViagem, itemCS.UltimaDataRecepcao.Value);

                    }
                    catch { }
                }
                await SelecionarViagem(DadosViagem, itemViagem);

            }
        }

        private async Task SelecionarViagem(UsuarioLogado usuarioLogado, Viagem item)
        {
            GlobalSetting.Instance.ViagemSelecionado = item;
            GlobalSetting.Instance.UsuarioLogado = usuarioLogado;
            EdicaoViagem = EdicaoDadoViagem = ViagemSelecionada = VisualizarGastos   = true;
            if (!usuarioLogado.IdentificadorViagem.HasValue)
            {
                EdicaoViagem = EdicaoDadoViagem = ViagemSelecionada = VisualizarGastos   = false;
            }
            else
            {
                ViagemSelecionada = true;
                EdicaoViagem = usuarioLogado.PermiteEdicao;
                EdicaoDadoViagem = usuarioLogado.Codigo == item.IdentificadorUsuario;
                if (EdicaoViagem)
                {
                    VisualizarGastos = true;
                    ViagemAberta = usuarioLogado.Aberto;
                    GPSIniciado = usuarioLogado.PermiteEdicao && usuarioLogado.Aberto && item.ControlaPosicaoGPS;
                }
                else
                { 
                    
                    ViagemAberta = false;
                    VisualizarGastos = usuarioLogado.VerCustos;
                    

                }
                if (Funcoes.AcessoInternet)
                {
                    GlobalSetting.Instance.AmigosViagem = new ObservableRangeCollection<Usuario>(await _apiService.CarregarParticipantesAmigo());
                }
                if (GPSIniciado.GetValueOrDefault())
                    await _gps.IniciarGPS();
                ViagemCarregada = true;
            }
        }



        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is UsuarioLogado item)
                ItemUsuario = item;
            Viagem itemViagem = await _database.GetViagemAtualAsync();
            if (itemViagem != null)
            {
                if (itemViagem.Edicao)
                {
                    var itemCS = await _database.GetControleSincronizacaoAsync();
                    itemCS.SincronizadoEnvio = false;
                    await _database.SalvarControleSincronizacao(itemCS);
                    //var itemUltimaPosicao = await _database.RetornarUltimaPosicao();
                }
                bool Online = Funcoes.AcessoInternet &&
                    await _apiService.VerificarOnLine();
                if (Online)
                {
                    if (!_itemUsuario.IdentificadorViagem.HasValue || _itemUsuario.IdentificadorViagem != itemViagem.Identificador)
                    {
                        var itemUsuario = await _apiService.SelecionarViagem(itemViagem.Identificador);
                        ItemUsuario.IdentificadorViagem = itemUsuario.IdentificadorViagem;
                        ItemUsuario.Aberto = itemUsuario.Aberto;
                        ItemUsuario.AuthenticationToken = itemUsuario.AuthenticationToken;
                        ItemUsuario.NomeViagem = itemUsuario.NomeViagem;
                        ItemUsuario.PermiteEdicao = itemUsuario.PermiteEdicao;
                        ItemUsuario.VerCustos = itemUsuario.VerCustos;
                    }
                    var viagemRemota = await _apiService.CarregarViagem(itemViagem.Identificador.GetValueOrDefault());
                    if (viagemRemota != null)     
                        _dataService.SincronizarParticipanteViagem(viagemRemota);
                    var task = _sincronizacao.Sincronizar(false);
                }
                else
                {
                    List<ParticipanteViagem> listaParticipantes = await _database.ListarParticipanteViagemAsync();
                    ItemUsuario.IdentificadorViagem = itemViagem.Identificador;
                    ItemUsuario.Aberto = itemViagem.Aberto;
                    ItemUsuario.PermiteEdicao = ItemUsuario.Codigo == itemViagem.IdentificadorUsuario || listaParticipantes.Where(d=>d.IdentificadorUsuario == ItemUsuario.Codigo).Any();
                }

                await SelecionarViagem(ItemUsuario, itemViagem); ;
            }
            else
            {
                EdicaoViagem = EdicaoDadoViagem = ViagemSelecionada = VisualizarGastos   = false;

            }
        }

        public UsuarioLogado ItemUsuario
        {
            get { return _itemUsuario; }
            set { SetProperty(ref _itemUsuario, value); }
        }
        public bool ViagemSelecionada
        {
            get
            {
                return _ViagemSelecionada;
            }
            set
            {
                SetProperty(ref _ViagemSelecionada, value);
            }
        }

        public bool? EditarViagem
        {
            get { return _editarViagem; }
            set { SetProperty(ref _editarViagem, value); }
        }

       
        public bool EdicaoViagem
        {
            get
            {
                return _EdicaoViagem;
            }
            set
            {
                SetProperty(ref _EdicaoViagem, value);
            }
        }

        public bool ViagemCarregada
        {
            get { return _ViagemCarregada; }
            set { SetProperty(ref _ViagemCarregada, value); }
        }
        public bool EdicaoDadoViagem
        {
            get
            {
                return _EdicaoDadoViagem;
            }
            set
            {
                SetProperty(ref _EdicaoDadoViagem, value);
            }
        }
        public bool VisualizarGastos
        {
            get
            {
                return _VisualizarGastos;
            }
            set
            {
                SetProperty(ref _VisualizarGastos, value);
            }
        }

        public bool? GPSIniciado
        {
            get
            {
                return _GPSIniciado;
            }
            set
            {
                SetProperty(ref _GPSIniciado, value);
            }
        }

        public bool? ViagemAberta
        {
            get
            {
                return _ViagemAberta;
            }
            set
            {
                SetProperty(ref _ViagemAberta, value);
            }
        }

        public ICommand LogoutCommand => new Command(async () =>
        {
            IsBusy = true;
            Xamarin.Essentials.SecureStorage.Remove( GlobalSetting.AppName);
            await _database.LimparBancoViagem();
            await NavigationService.TrocarPaginaShell("//LoginPage");
            IsBusy = false;
        });

        public ICommand TrocarGPSCommand => new Command(async () =>
        {
            if (ViagemAberta.GetValueOrDefault() && EdicaoViagem)
            {
                if (GPSIniciado.GetValueOrDefault())
                {
                    await _gps.PararGPS();
                }
                else
                    await _gps.IniciarGPS();
                GPSIniciado = !GPSIniciado;
                GlobalSetting.Instance.ViagemSelecionado.ControlaPosicaoGPS = GPSIniciado.GetValueOrDefault();
                await _database.SalvarViagemAsync(GlobalSetting.Instance.ViagemSelecionado);
            }
        });
        public ICommand TrocarSituacaoCommand => new Command(async () =>
        {
            if ( EdicaoViagem)
            {
                if (ViagemAberta.GetValueOrDefault())
                {
                    await _gps.PararGPS();
                    GPSIniciado = false;
                }
                else
                    await _gps.IniciarGPS();

                ViagemAberta = !ViagemAberta;
                
                GlobalSetting.Instance.ViagemSelecionado.Aberto = ViagemAberta.GetValueOrDefault();
                GlobalSetting.Instance.ViagemSelecionado.ControlaPosicaoGPS = GPSIniciado.GetValueOrDefault();
                if (Funcoes.AcessoInternet)
                {
                    await _apiService.SalvarViagemSimples(GlobalSetting.Instance.ViagemSelecionado);
                }
                await _database.SalvarViagemAsync(GlobalSetting.Instance.ViagemSelecionado);

            }
        });

        public ICommand SincronizarCommand => new Command(async () =>
        {
            if (EdicaoViagem)
            {
                if (!Funcoes.AcessoInternet)
                    await DialogService.ShowAlertAsync(AppResource.ConexaoInternetObrigatoria, AppResource.Sincronizacao, AppResource.Ok);
                else
                    await NavigationService.TrocarPaginaShell("SincronizacaoPage");
            }

        });

        public ICommand FotosCommand => new Command(async () =>
        {
            string[] opcoes = new string[] { AppResource.Foto, AppResource.Video };
            var acao = await DialogService.ShowActionList(AppResource.TipoMidia, AppResource.Cancelar, string.Empty, opcoes.ToList());

            if (acao == AppResource.Foto || acao == AppResource.Video)
            {
                UploadFoto itemUpload = new UploadFoto() { Video = acao == AppResource.Video, ImageMime = acao == AppResource.Video ? "video/avi" : "image/jpeg" };
                Task task = AtualizarPosicao(itemUpload);
                
                FileResult file = null;
                if (itemUpload.Video)
                    file = await MediaPicker.CaptureVideoAsync();
                else
                    file = await MediaPicker.CapturePhotoAsync();                
                if (file != null)
                {
                    itemUpload.DataArquivo = DateTime.Now;
                    itemUpload.file = file;
                    itemUpload.CaminhoLocal = file.FullPath;
                    SalvarFoto(itemUpload);
                }

            }
        });

        private async void SalvarFoto(UploadFoto itemUpload)
        {
            
                string configuracao = itemUpload.Video ? _settingsService.ModoVideo : _settingsService.ModoImagem;
                //imagemUpload.Comentario = Texto.Value;
                if (Funcoes.AcessoInternet && (configuracao == "1" || (configuracao == "2" && Funcoes.AcessoRede)))
                {
                    UploadFoto(itemUpload);
                }
                else
                {
                    await _database.SalvarUploadFoto(itemUpload);
                }
                

            }

        private async void UploadFoto(UploadFoto itemUpload)
        {
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            await _accountSevice.AtualizarToken();

            var stream = await itemUpload.file.OpenReadAsync();
            using (var newStream = new MemoryStream())
            {
                await stream.CopyToAsync(newStream);
                byte[] DadosFoto = newStream.ToArray();

                await _foto.SubirFoto(usuario.Properties["access_token"], GlobalSetting.Instance.ViagemSelecionado.CodigoAlbum, DadosFoto, itemUpload);
                await _apiService.SubirImagem(itemUpload);

            }
        }
    

        private async Task AtualizarPosicao(UploadFoto itemUpload)
        {
            var posicao = await _gps.RetornarPosicao();
            if (posicao != null)
            {
                itemUpload.Latitude = posicao.Latitude;
                itemUpload.Longitude = posicao.Longitude;
            }
        }
    }
}

