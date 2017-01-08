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
    public class ListagemSugestaoViewModel: BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Sugestao _ItemSelecionado;


        public ListagemSugestaoViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca();
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
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

            ExcluirCommand = new Command<Sugestao>((item) => Excluir(item));
            EditarCommand = new Command<ItemTappedEventArgs>(async (item) => await Editar(item));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);
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
        public Command AdicionarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }

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
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarCidadeSugestao();
                ListaCidades = new ObservableCollection<Cidade>(Dados);
                OnPropertyChanged("ListaCidades");
            }
        }


        private async Task CarregarListaPedidos()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarSugestao(ItemCriterioBusca);
                ListaDados = new ObservableCollection<Sugestao>(Dados);
                OnPropertyChanged("ListaDados");
            }
            IsLoadingLista = false;
        }

        private void Excluir(Sugestao item)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir a sugestão {0}?", item.Local),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.ExcluirSugestao(item.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                        {
                            var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                            ListaDados.RemoveAt(Posicao);
                        }
                    }

                })
            });
        }

        private async Task Editar(ItemTappedEventArgs item)
        {
            using (ApiService srv = new ApiService())
            {
                var itemEditar = await srv.CarregarSugestao(((Sugestao)item.Item).Identificador);
                var pagina = new EdicaoSugestaoPage() { BindingContext = new EdicaoSugestaoViewModel(itemEditar) };
                await PushAsync(pagina);
            }
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new Sugestao() { IdentificadorUsuario = ItemUsuarioLogado.Codigo, Status = 0};
            var pagina = new EdicaoSugestaoPage() { BindingContext = new EdicaoSugestaoViewModel(itemEditar) };
            await PushAsync(pagina);

        }
    }
}
