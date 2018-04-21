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
    public class ListagemSugestaoRecebidaViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Sugestao _ItemSelecionado;


        public ListagemSugestaoRecebidaViewModel(Viagem pitemViagem)
        {
            IsLoadingLista = true;
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { Situacao=1};
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaAmigos();
                                                                       await CarregarListaCidades();
                                                                       await CarregarListaPedidos();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaPedidos();
                                                      },
                                                      () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
            ListaStatus = new ObservableCollection<ItemLista>();
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Pendente" });
            ListaStatus.Add(new ItemLista() { Codigo = "2", Descricao = "Agendada" });
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Ignorada" });
            ListaStatus.Add(new ItemLista() { Codigo = "-1", Descricao = "Todas" });

            MessagingService.Current.Unsubscribe<Sugestao>(MessageKeys.ManutencaoSugestao);
            MessagingService.Current.Subscribe<Sugestao>(MessageKeys.ManutencaoSugestao, (service, item) =>
            {
                IsBusy = true;

                if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                {
                    var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                    ListaDados.RemoveAt(Posicao);
                    ListaDados.Insert(Posicao, item);
                }
                else
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
        public ObservableCollection<Usuario> ListaAmigos { get; set; }
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


        public ObservableCollection<Sugestao> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }

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

        public Sugestao ItemSelecionado
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
                await CarregarListaPedidos();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;

        }

        private async Task CarregarListaCidades()
        {
            List<Cidade> Dados = new List<Cidade>();
            bool Executado = true;
            if (Conectado)
            {
                try { 
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarCidadeSugestao();

                }
                }
                catch { Executado = false; }
            }
            if (!Executado)
            {
                Dados = await DatabaseService.Database.ListarCidade_Tipo("S");
            }
            ListaCidades = new ObservableCollection<Cidade>(Dados);
            OnPropertyChanged("ListaCidades");
        }
        private async Task CarregarListaAmigos()
        {
            List<Usuario> Dados = new List<Usuario>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarAmigos();
                    if (!Dados.Any())
                        Dados = await DatabaseService.Database.ListarAmigos();
                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarAmigos();
            }
            ListaAmigos = new ObservableCollection<Usuario>(Dados);
            OnPropertyChanged("ListaAmigos");
        }

        private async Task CarregarListaPedidos()
        {
            List<Sugestao> Dados = new List<Sugestao>();
            bool Executado = true;
            if (Conectado)
            {
                try { 
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarSugestaoRecebida(ItemCriterioBusca);

                }
                }
                catch { Executado = false; }
            }
            if (!Executado)
                Dados = await DatabaseService.Database.ListarSugestao(ItemCriterioBusca);

            ListaDados = new ObservableCollection<Sugestao>(Dados);
            OnPropertyChanged("ListaDados");
            await Task.Delay(100);
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            var Pagina = new EdicaoSugestaoRecebidaPage() { BindingContext = new EdicaoSugestaoRecebidaViewModel((Sugestao)itemSelecionado.Item) };
            await PushAsync(Pagina);
        }

      


    }
}
