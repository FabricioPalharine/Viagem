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

namespace CV.Mobile.ViewModels
{
    public class ListagemAtracaoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Atracao _ItemSelecionado;


        public ListagemAtracaoViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { Situacao=1};
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaCidades();
                                                                       await CarregarListaDados();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
            ListaStatus = new ObservableCollection<ItemLista>();
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Visitando" });
            ListaStatus.Add(new ItemLista() { Codigo = "2", Descricao = "Visitada" });
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Não Iniciada" });
            ListaStatus.Add(new ItemLista() { Codigo = "4", Descricao = "Todas" });

            MessagingService.Current.Subscribe<Atracao>(MessageKeys.ManutencaoAtracao, (service, item) =>
            {
                IsBusy = true;
                
                if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                {
                    var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                    ListaDados.RemoveAt(Posicao);
                    if (!item.DataExclusao.HasValue)
                        ListaDados.Insert(Posicao, item);
                }
                else if (!item.DataExclusao.HasValue)
                    ListaDados.Add(item);

                IsBusy = false;
            });
        }

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

        public Viagem ItemViagem { get; set; }
        public ObservableCollection<Cidade> ListaCidades { get; set; }
        public ObservableCollection<ItemLista> ListaStatus { get; set; }

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


        public ObservableCollection<Atracao> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

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

        public Atracao ItemSelecionado
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


        private async Task VerificarPesquisa()
        {
            if (ModoPesquisa)
            {
                if (PesquisarCommand.CanExecute(null))
                    PesquisarCommand.ChangeCanExecute();
                await CarregarListaDados();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;

        }

        private async Task CarregarListaCidades()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarCidadeSugestao();
                ListaCidades = new ObservableCollection<Cidade>(Dados);
                OnPropertyChanged("ListaCidades");
            }
        }
      

        private async Task CarregarListaDados()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarAtracao(ItemCriterioBusca);
                ListaDados = new ObservableCollection<Atracao>(Dados);
                OnPropertyChanged("ListaDados");
            }
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            using (ApiService srv = new ApiService())
            {
                var ItemAtracao = await srv.CarregarAtracao(((Atracao)itemSelecionado.Item).Identificador);
                var Pagina = new EdicaoAtracaoPage() { BindingContext = new EdicaoAtracaoViewModel(ItemAtracao,ItemViagem) };
                await PushAsync(Pagina);
            }
        }
        private async Task Adicionar()
        {
            var ItemAtracao = new Atracao() { Avaliacoes = new ObservableCollection<AvaliacaoAtracao>() } ;
            using (ApiService srv = new ApiService())
            {
                var AtracaoAberto = await srv.VerificarAtracaoAberto();
                if (AtracaoAberto != null)
                {
                    MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
                    {
                        Title = "Confirmação",
                        Question = String.Format("A atração {0} está sendo visitada, deseja associar a nova atração como filha dela?", AtracaoAberto.Nome),
                        Positive = "Sim",
                        Negative = "Não",
                        OnCompleted = new Action<bool>(async result =>
                        {
                            if (result)
                            {
                                ItemAtracao.IdentificadorAtracaoPai = AtracaoAberto.Identificador;
                            }
                            var Pagina2 = new EdicaoAtracaoPage() { BindingContext = new EdicaoAtracaoViewModel(ItemAtracao, ItemViagem) };
                            await PushAsync(Pagina2);


                        })
                    });
                }
                else
                {
                    var Pagina = new EdicaoAtracaoPage() { BindingContext = new EdicaoAtracaoViewModel(ItemAtracao, ItemViagem) };
                    await PushAsync(Pagina);
                }
            }
        }

    }
}
