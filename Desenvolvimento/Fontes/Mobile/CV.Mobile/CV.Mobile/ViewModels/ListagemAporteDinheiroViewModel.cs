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
    public class ListagemAporteDinheiroViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;

        private AporteDinheiro _ItemSelecionado;



        public ListagemAporteDinheiroViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { } ;
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaDados();

                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);


            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

          
            ExcluirCommand = new Command<AporteDinheiro>((item) => Excluir(item));
            EditarCommand = new Command<ItemTappedEventArgs>(async (item) => await Editar(item));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);

            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));

            MessagingService.Current.Unsubscribe<AporteDinheiro>(MessageKeys.ManutencaoAporteDinheiro);
            MessagingService.Current.Subscribe<AporteDinheiro>(MessageKeys.ManutencaoAporteDinheiro, (service, item) =>
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
        public ObservableCollection<ItemLista> ListaMoeda { get; set; }


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


        public ObservableCollection<AporteDinheiro> ListaDados { get; set; }
  
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


        public AporteDinheiro ItemSelecionado
        {
            get
            {
                return _ItemSelecionado;
            }

            set
            {
                SetProperty(ref _ItemSelecionado, null);
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

     

        private async Task CarregarListaDados()
        {
            List<AporteDinheiro> Dados = new List<AporteDinheiro>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarAporteDinheiro(ItemCriterioBusca);
                }
            }
            else
                Dados = await DatabaseService.Database.ListarAporteDinheiro(ItemCriterioBusca);
            ListaDados = new ObservableCollection<AporteDinheiro>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }

    

        private void Excluir(AporteDinheiro item)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir a compra de {0} {1}", item.Valor.GetValueOrDefault(0).ToString("N2"), item.MoedaSigla),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirAporteDinheiro(item.Identificador);
                            base.AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "AD", item.Identificador.GetValueOrDefault(), false);
                            await DatabaseService.ExcluirAporteDinheiro(item,true);

                        }
                    }
                    else
                    {
                       
                        await DatabaseService.ExcluirAporteDinheiro(item, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Aporte Dinheiro Gravado com Sucesso " } };
                    }
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
                })
            });
        }

        private async Task Editar(ItemTappedEventArgs itemLista)
        {
            AporteDinheiro itemEditar = null;
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    itemEditar = await srv.CarregarAporteDinheiro(((AporteDinheiro)itemLista.Item).Identificador);
                }
            }
            else

                itemEditar = await DatabaseService.CarregarAporteDinheiro(((AporteDinheiro)itemLista.Item).Identificador);
                var pagina = new EdicaoAporteDinheiro() { BindingContext = new EdicaoAporteDinheiroViewModel(itemEditar) };
                await PushAsync(pagina);
            
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new AporteDinheiro() { DataAporte=DateTime.Today, Moeda = ItemViagem.Moeda };
            var pagina = new EdicaoAporteDinheiro() { BindingContext = new EdicaoAporteDinheiroViewModel(itemEditar) };
            await PushAsync(pagina);

        }
    }
}
