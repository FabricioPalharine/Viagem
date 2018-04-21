using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using CV.Mobile.Helpers;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers;
using Plugin.Share;

namespace CV.Mobile.ViewModels
{
    public class ConsultarTimelineViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _IsLoadingLista;
        private Timeline _ItemSelecionado;
        private bool _ModoPesquisa;
        private bool _CarregandoMais = false;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        public ConsultarTimelineViewModel()
        {
            ItemCriterioBusca = new CriterioBusca() { Count = 10 };

            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        if (ModoPesquisa)
                                                                            await CarregarListaDados();
                                                                        ModoPesquisa = !ModoPesquisa;
                                                                    },
                                                                    () => true);

            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            MessagingService.Current.Unsubscribe<AtualizacaoConsulta>(MessageKeys.VerificarCalendario);
            MessagingService.Current.Subscribe<AtualizacaoConsulta>(MessageKeys.AtualizarConsulta, async (service, item) =>
            {
                if (item.Tipo != "P" && !ItemCriterioBusca.DataFimAte.HasValue)
                {
                    var filtroLocal = ItemCriterioBusca.Clone();
                    filtroLocal.DataInicioDe = null;
                    filtroLocal.Count = 500;
                    filtroLocal.Tipo = item.Tipo;
                    filtroLocal.Identificador = item.Identificador;
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            var listaRetorno = await srv.ConsultarTimeline(filtroLocal);
                            foreach (var itemnovo in listaRetorno.OrderBy(d => d.Data))
                            {
                                if (!ListaDados.Where(d => d.Identificador == itemnovo.Identificador && d.Tipo == itemnovo.Tipo).Any())
                                {
                                    ListaDados.Insert(0, itemnovo);
                                }
                            }

                        }
                    }
                    catch
                    {
                        ApiService.ExibirMensagemErro();
                    }

                }
            }
           );
        }


        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                async () =>
                                {
                                    if (ListaUsuario == null)
                                    {
                                        await CarregarListaUsuarios();
                                        IsLoadingLista = true;

                                        if (ListaDados == null)
                                            await CarregarListaDados();
                                    }
                                },
                                () => true);
            }
        }

        private async Task CarregarListaUsuarios()
        {
            if (ListaUsuario == null)
            {
                using (ApiService srv = new ApiService())
                {
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.CarregarParticipantesAmigo());
                    OnPropertyChanged("ListaUsuario");
                }
            }
        }

        public ObservableCollection<ItemLista> ListaTipo { get; set; }
        public CriterioBusca ItemCriterioBusca
        {
            get
            {
                return _itemCriterioBusca;
            }

            set
            {
                SetProperty(ref _itemCriterioBusca, value);
            }
        }


        public ObservableCollection<Timeline> ListaDados { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemAppearingCommand
        {
            get
            {
                return new Command<ItemVisibilityEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
            }
        }

        public Command AbrirMapaCommand {
            get
            {
                return
              new Command<Timeline>(async (item) => await AbrirMapa(item));
            }
        }

        public Command AbrirYoutubeCommand
        {
            get
            {
                return
              new Command<Timeline>(async (item) =>
              {
                  try
                  {
                      using (ApiService srv = new ApiService())
                      {
                          var itemFoto = await srv.CarregarFoto(item.Identificador);
                          Device.OpenUri(new Uri(string.Concat("https://www.youtube.com/watch?v=", itemFoto.CodigoFoto)));
                      }
                  }
                  catch
                  {
                      ApiService.ExibirMensagemErro();
                  }

              });
            }
        }

        private async Task AbrirMapa(Timeline item)
        {
            PosicaoMapaViewModel vm = new PosicaoMapaViewModel(new Xamarin.Forms.Maps.Position(item.Latitude.GetValueOrDefault(), item.Longitude.GetValueOrDefault()));
            Page pagina = new PosicaoMapaPage() { BindingContext = vm };
            await PushAsync(pagina);
        }

        private async Task VerificarAcaoItem(ItemVisibilityEventArgs obj)
        {
            Timeline itemTimeline = (Timeline)obj.Item;
            var Posicao = ListaDados.IndexOf(itemTimeline);
            if (Posicao > ListaDados.Count() - 3 && !_CarregandoMais)
            {
                IsLoadingLista = true;
                _CarregandoMais = true;
                ItemCriterioBusca.DataInicioDe = ListaDados.Select(d => d.Data).LastOrDefault();
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        var ListaAdicoes = await srv.ConsultarTimeline(ItemCriterioBusca);
                        foreach (var item in ListaAdicoes.Where(d => d.Tipo == "Video"))
                        {
                            item.UrlThumbnail = (await srv.CarregarFoto(item.Identificador)).LinkThumbnail;
                        }
                        foreach (var item in ListaAdicoes)
                            ListaDados.Add(item);
                    }
                }
                catch
                {
                    ApiService.ExibirMensagemErro();
                }
                IsLoadingLista = false;
                _CarregandoMais = false;
            }
        }

        public bool IsLoadingLista
        {
            get
            {
                return _IsLoadingLista;
            }

            set
            {
                SetProperty(ref _IsLoadingLista, value);
            }
        }

        public bool ModoPesquisa
        {
            get
            {
                return _ModoPesquisa;
            }

            set
            {
                SetProperty(ref _ModoPesquisa, value);
            }
        }

        public Timeline ItemSelecionado
        {
            get
            {
                return _ItemSelecionado;
            }

            set
            {
                _ItemSelecionado = null;
                OnPropertyChanged();
            }
        }

        private async Task CarregarListaDados()
        {
            List<Timeline> Dados = new List<Timeline>();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ConsultarTimeline(ItemCriterioBusca);
                    foreach (var item in Dados.Where(d => d.Tipo == "Video"))
                    {
                        item.UrlThumbnail = (await srv.CarregarFoto(item.Identificador)).LinkThumbnail;
                    }
                }
            }
            catch
            {
                ApiService.ExibirMensagemErro();    
            }

            ListaDados = new ObservableCollection<Timeline>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }


    }
}
