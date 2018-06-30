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
    public class ListagemLojaViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Loja _ItemSelecionado;


        public ListagemLojaViewModel(Viagem pitemViagem)
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

            MessagingService.Current.Unsubscribe<Loja>(MessageKeys.ManutencaoLoja);
            MessagingService.Current.Subscribe<Loja>(MessageKeys.ManutencaoLoja, (service, item) =>
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


        public ObservableCollection<Loja> ListaDados { get; set; }
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

        public Loja ItemSelecionado
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
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarCidadeLoja();

                    }
                    Executado = true;
                }
                catch { Executado = false; }

            }
            if (!Executado)
            {
                Dados = await DatabaseService.Database.ListarCidade_Tipo("L");
            }
            ListaCidades = new ObservableCollection<Cidade>(Dados);
            OnPropertyChanged("ListaCidades");


        }


        private async Task CarregarListaDados()
        {
            List<Loja> Dados = new List<Loja>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarLoja(ItemCriterioBusca);

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
            {
                Dados = await DatabaseService.Database.ListarLoja(ItemCriterioBusca);
            }
            ListaDados = new ObservableCollection<Loja>(Dados);
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;

        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {

            Loja ItemLoja = null;
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ItemLoja = await srv.CarregarLoja(((Loja)itemSelecionado.Item).Identificador);

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
                ItemLoja = await DatabaseService.CarregarLoja(((Loja)itemSelecionado.Item).Identificador);


            var Pagina = new EdicaoLojaPage() { BindingContext = new EdicaoLojaViewModel(ItemLoja, ItemViagem) };
            await PushAsync(Pagina);

        }
        private async Task Adicionar()
        {
            var ItemLoja = new Loja() { Avaliacoes = new ObservableRangeCollection<AvaliacaoLoja>() };
            bool Executado = false;
            Atracao AtracaoAberto = null;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        AtracaoAberto = await srv.VerificarAtracaoAberto();
                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
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
                            ItemLoja.IdentificadorAtracao = AtracaoAberto.Identificador;
                        }
                        var Pagina = new EdicaoLojaPage() { BindingContext = new EdicaoLojaViewModel(ItemLoja, ItemViagem) };
                        await PushAsync(Pagina);


                    })
                });
            }
            else
            {
                var Pagina = new EdicaoLojaPage() { BindingContext = new EdicaoLojaViewModel(ItemLoja, ItemViagem) };
                await PushAsync(Pagina);
            }
        }




    }
}
