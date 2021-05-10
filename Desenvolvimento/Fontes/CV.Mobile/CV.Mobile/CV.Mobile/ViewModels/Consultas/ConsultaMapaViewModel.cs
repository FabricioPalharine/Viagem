using CV.Mobile.Controls;
using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Fotos;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels.Consultas
{
    public class ConsultaMapaViewModel : BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { };
        private readonly IApiService _apiService;
        private ObservableCollection<CustomPin> _pontos = new ObservableCollection<CustomPin>();
        private ObservableCollection<Polyline> _linhas = new ObservableCollection<Polyline>();
        private readonly ISettingsService _settingsService;
        private readonly IFoto _foto;
        public ConsultaMapaViewModel(IApiService apiService, ISettingsService settingsService, IFoto foto)
        {
            _apiService = apiService;
            _foto = foto;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<ConsultaMapaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarMapa);
                MessagingCenter.Subscribe<ConsultaMapaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarMapa, (sender, obj) =>
                {

                    criterioBusca.DataInicioDe = obj.DataInicioDe;
                    criterioBusca.DataInicioAte = obj.DataInicioAte;
                    criterioBusca.Tipo = obj.Tipo;
                    criterioBusca.IdentificadorParticipante = obj.IdentificadorParticipante;
                });
                if (!criterioBusca.IdentificadorParticipante.HasValue)
                {
                    ObservableCollection<Usuario> Usuarios = new ObservableCollection<Usuario>();
                    if (GlobalSetting.Instance.AmigosViagem == null)
                        GlobalSetting.Instance.AmigosViagem = Usuarios = new ObservableCollection<Usuario>(await _apiService.CarregarParticipantesAmigo());
                    else
                        Usuarios = new ObservableCollection<Usuario>(GlobalSetting.Instance.AmigosViagem);
                    if (Usuarios.Where(d => d.Identificador == GlobalSetting.Instance.UsuarioLogado.Codigo).Any())
                        criterioBusca.IdentificadorParticipante = GlobalSetting.Instance.UsuarioLogado.Codigo;
                    else
                        criterioBusca.IdentificadorParticipante = Usuarios.Select(d => d.Identificador).FirstOrDefault();
                }
                await CarregarLista();
            }
            else
            {
                await DialogService.ShowAlertAsync(AppResource.AplicacaoOnlineNecessaria, AppResource.AppName, AppResource.Ok);
                await NavigationService.TrocarPaginaShell("..");
            }
        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ConsultaMapaFiltroPage", criterioBusca);

        });


        public ObservableCollection<CustomPin> Pontos
        {
            get { return _pontos; }
            set { SetProperty(ref _pontos, value); }
        }

        public ObservableCollection<Polyline> Linhas
        {
            get { return _linhas; }
            set { SetProperty(ref _linhas, value); }
        }


        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                Pontos = new ObservableCollection<CustomPin>();
                IList<PontoMapa> lista = await _apiService.ListarPontosViagem(criterioBusca);
                IList<LinhaMapa> linhas = await _apiService.ListarLinhasViagem(criterioBusca);

                if (criterioBusca.Tipo == "F")
                {
                    var pontosFoto = lista.Where(d => !string.IsNullOrEmpty(d.NomeArquivo)).ToList();
                    if (pontosFoto.Any())
                    {
                        await _foto.UpdateMediaData(pontosFoto);
                    }
                    foreach (var ponto in lista.Where(d=>d.Tipo == "F"))
                    {
                        var pin = new CustomPin()
                        {
                            Label = ponto.Nome ?? ponto.Tipo,
                            Position = new Position(ponto.Latitude.Value, ponto.Longitude.Value),
                            Type = PinType.SearchResult
                        };

                        var url = ponto.UrlTumbnail;
                        pin.ImageSource = ImageSource.FromUri(new Uri(url));

                        Pontos.Add(pin);
                    }
                }
                else
                {
                    foreach (var ponto in lista)
                    {
                        var pin = new CustomPin()
                        {
                            Label = ponto.Nome ?? ponto.Tipo,
                            Position = new Position(ponto.Latitude.Value, ponto.Longitude.Value),
                            Type = PinType.SearchResult
                        };
                        //var fontFamily = Convert.ToString(((Xamarin.Forms.OnPlatform<string>)App.Current.Resources["MaterialFontFamily"]).Platforms.Where(d => d.Platform.Contains(Device.RuntimePlatform)).Select(d => d.Value).FirstOrDefault());
                        if (ponto.Tipo == "A")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinAtracao.png");
                        else if (ponto.Tipo == "H")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinHotel.png");
                        else if (ponto.Tipo == "R")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinRestaurante.png");
                        else if (ponto.Tipo == "P")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinParada.png");
                        else if (ponto.Tipo == "T")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinComentario.png");
                        else if (ponto.Tipo == "U")
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinAtual.png");
                        else if (ponto.Tipo == "F" || ponto.Tipo == "V")
                        {
                            pin.ImageSource = ImageSource.FromResource("CV.Mobile.Resources.pinFoto.png");
                        }
                        Pontos.Add(pin);
                    }                  
                    

                }
                if (lista.Any())
                {
                    var latitudeMedia = lista.Average(d => d.Latitude);
                    var longitudeMedia = lista.Average(d => d.Longitude);
                    MessagingCenter.Send<ConsultaMapaViewModel, Position>(this, MessageKeys.CentralizarMapa, new Position(latitudeMedia.GetValueOrDefault(), longitudeMedia.GetValueOrDefault()));
                }
                Linhas = new ObservableCollection<Polyline>();
                if(linhas.Any())
                {
                    foreach(var linha in linhas)
                    {
                        Polyline polyline = new Polyline()
                        {
                            StrokeColor = linha.Tipo == "N" ? Color.Blue : linha.Tipo == "D" ? Color.Green : Color.Yellow
                        };
                        foreach (var posicao in linha.Pontos)
                        {
                            polyline.Geopath.Add(new Position(posicao.Latitude.GetValueOrDefault(), posicao.Longitude.GetValueOrDefault())) ;
                        }
                        Linhas.Add(polyline);
                    }
                }
               
                     
                

            }
            finally
            {
                IsBusy = false;
            }
        }
    }

}
