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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Consultas
{
    public class TimelineViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { Situacao = 1, Count=20 };
        private readonly IApiService _apiService;
        private ObservableCollection<Timeline> _dados = new ObservableCollection<Timeline>();
        private readonly ISettingsService _settingsService;
        private readonly IFoto _foto;
        private bool _carregado = false;
        private bool _carregando = false;
        public TimelineViewModel(IApiService apiService, ISettingsService settingsService, IFoto foto)
        {
            _apiService = apiService;
            _foto = foto;
            criterioBusca.DataInicioAte = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<TimelineFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarTimeline);
                MessagingCenter.Subscribe<TimelineFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarTimeline,  async (sender, obj) =>
                {

                    criterioBusca.IdentificadorParticipante = obj.IdentificadorParticipante;
                    criterioBusca.DataInicioAte = obj.DataInicioAte;
                    criterioBusca.DataInicioDe = null;
                    await CarregarLista();

                });

                Device.StartTimer(TimeSpan.FromMinutes(5), ()=>
                {
                    VerificarNovosItens();
                    return true;
                });
                if (!_carregado)
                await CarregarLista();
            }
            else
            {
                await DialogService.ShowAlertAsync(AppResource.AplicacaoOnlineNecessaria, AppResource.AppName, AppResource.Ok);
                await NavigationService.TrocarPaginaShell("..");
            }
        });

        public ICommand RecarregarListaCommand => new Command(() =>
        {
            VerificarNovosItens();

        });

        public ICommand CarregarMaisCommand => new Command<ItemVisibilityEventArgs>(async (obj) => await VerificarAcaoItem(obj));



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("TimelineFiltroPage", criterioBusca);

        });

        public ICommand AbrirImagemCommand => new Command<Timeline>(async (d) =>
        {
            var foto = new Foto() { CodigoFoto = d.GoogleId, Comentario = d.Comentario, Data = d.Data, Video = d.LinhaVideo, LinkFoto = d.Url, LinkThumbnail = d.UrlThumbnail };
            await NavigationService.TrocarPaginaShell("ConsultaFotoDetalhePage", foto);
        });

        public ObservableCollection<Timeline> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Timeline> lista = await _apiService.ConsultarTimeline(criterioBusca);
                await CarregarLinksFotos(lista);

                Dados = new ObservableCollection<Timeline>(lista);
                _carregado = true;
            }            
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CarregarLinksFotos(IList<Timeline> lista)
        {
            var listaImagens = lista.Where(d => d.LinhaFoto || d.LinhaVideo).ToList();
            if (listaImagens.Any())
                await _foto.UpdateMediaData(listaImagens);
        }

        private async Task AbrirMapa(Timeline item)
        {
            PosicaoMapa posicao = new PosicaoMapa() { Latitude = item.Latitude.GetValueOrDefault(), Longitude = item.Longitude.GetValueOrDefault() };
            await NavigationService.TrocarPaginaShell("MapaExibicaoPage", posicao);
        }

        private async void VerificarNovosItens()
        {
            var criterio = criterioBusca.Clone();
            criterio.DataInicioDe = null;
            criterio.DataInicioAte = Dados.Any() ? Dados.Max(d => d.Data) : new Nullable<DateTime>();
            criterio.Count = int.MaxValue;
            IsBusy = true;
            _carregando = true;
            try
            {

                var ListaAdicoes = await _apiService.ConsultarTimeline(criterioBusca);
                await CarregarLinksFotos(ListaAdicoes);
                foreach (var item in ListaAdicoes.OrderBy(d=>d.Data))
                    Dados.Insert(0,item);

            }
            finally
            {
                IsBusy = false;
                _carregando = false;
            }
        }

        private async Task VerificarAcaoItem(ItemVisibilityEventArgs obj)
        {
            Timeline itemTimeline = (Timeline)obj.Item;
            var Posicao = Dados.IndexOf(itemTimeline);
            if (Posicao > Dados.Count() - 3 && !_carregando)
            {
                IsBusy = true;
                _carregando = true;
                criterioBusca.DataInicioDe = Dados.Any()? Dados.Min(d => d.Data): new Nullable<DateTime>();
                try
                {
                    
                        var ListaAdicoes = await _apiService.ConsultarTimeline(criterioBusca);
                    await CarregarLinksFotos(ListaAdicoes);

                    foreach (var item in ListaAdicoes)
                            Dados.Add(item);
                    
                }
                finally
                {
                    IsBusy = false;
                    _carregando = false;
                }
            }
        }
    }
}
