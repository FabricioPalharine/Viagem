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

namespace CV.Mobile.ViewModels
{
    public class ListagemRefeicaoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Refeicao _ItemSelecionado;


        public ListagemRefeicaoViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { };
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

            MessagingService.Current.Unsubscribe<Refeicao>(MessageKeys.ManutencaoRefeicao);
            MessagingService.Current.Subscribe<Refeicao>(MessageKeys.ManutencaoRefeicao, (service, item) =>
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


        public ObservableCollection<Refeicao> ListaDados { get; set; }
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

        public Refeicao ItemSelecionado
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

            List<Cidade> Dados = new List<Cidade>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarCidadeRefeicao();

                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarCidade_Tipo("R");
            }
            ListaCidades = new ObservableCollection<Cidade>(Dados);
            OnPropertyChanged("ListaCidades");

          
        }
      

        private async Task CarregarListaDados()
        {
            List<Refeicao> Dados = new List<Refeicao>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarRefeicao(ItemCriterioBusca);

                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarRefeicao(ItemCriterioBusca);
            }
            ListaDados = new ObservableCollection<Refeicao>(Dados);
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;

            
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {

            Refeicao ItemRefeicao = null;
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    ItemRefeicao = await srv.CarregarRefeicao(((Refeicao)itemSelecionado.Item).Identificador);

                }
            }
            else
                ItemRefeicao = await DatabaseService.CarregarRefeicao(((Refeicao)itemSelecionado.Item).Identificador);

              var Pagina = new EdicaoRefeicaoPage() { BindingContext = new EdicaoRefeicaoViewModel(ItemRefeicao,ItemViagem) };
                await PushAsync(Pagina);
            
        }
        private async Task Adicionar()
        {
            var ItemRefeicao = new Refeicao() { Pedidos = new ObservableRangeCollection<RefeicaoPedido>(), Data = DateTime.Now, Hora = DateTime.Now.TimeOfDay };
            Atracao AtracaoAberto = null;
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    AtracaoAberto = await srv.VerificarAtracaoAberto();
                }
            }
            else
            {
                AtracaoAberto = await DatabaseService.Database.RetornarAtracaoAberta();
            }
            if (AtracaoAberto != null)
            {
                MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
                {
                    Title = "Confirmação",
                    Question = String.Format("A atração {0} está sendo visitada, deseja associar a refeição como filha dela?", AtracaoAberto.Nome),
                    Positive = "Sim",
                    Negative = "Não",
                    OnCompleted = new Action<bool>(async result =>
                    {
                        if (result)
                        {
                            ItemRefeicao.IdentificadorAtracao = AtracaoAberto.Identificador;
                        }
                        var Pagina = new EdicaoRefeicaoPage() { BindingContext = new EdicaoRefeicaoViewModel(ItemRefeicao, ItemViagem) };
                        await PushAsync(Pagina);


                    })
                });
            }
            else
            {
                var Pagina = new EdicaoRefeicaoPage() { BindingContext = new EdicaoRefeicaoViewModel(ItemRefeicao, ItemViagem) };
                await PushAsync(Pagina);
            }
        }

    }
}
