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
    public class ListagemViagemAereaViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private ViagemAerea _ItemSelecionado;


        public ListagemViagemAereaViewModel(Viagem pitemViagem)
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
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Viajando" });
            ListaStatus.Add(new ItemLista() { Codigo = "2", Descricao = "Terminada" });
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Agendada" });
            ListaStatus.Add(new ItemLista() { Codigo = "4", Descricao = "Todas" });

            ListaTipo = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumTipoTransporte)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumTipoTransporte)enumerador).Descricao() };
                ListaTipo.Add(item);
            }
            ListaTipo = new ObservableCollection<ItemLista>(ListaTipo.OrderBy(d => d.Descricao));

            MessagingService.Current.Unsubscribe<ViagemAerea>(MessageKeys.ManutencaoViagemAerea);
            MessagingService.Current.Subscribe<ViagemAerea>(MessageKeys.ManutencaoViagemAerea, (service, item) =>
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
        public ObservableCollection<ItemLista> ListaTipo { get; set; }

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


        public ObservableCollection<ViagemAerea> ListaDados { get; set; }
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

        public ViagemAerea ItemSelecionado
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
                    Dados = await srv.ListarCidadeViagemAerea();
                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarCidade_Tipo("V");
            }
            ListaCidades = new ObservableCollection<Cidade>(Dados);
            OnPropertyChanged("ListaCidades");

        }


        private async Task CarregarListaDados()
        {
            List<ViagemAerea> Dados = new List<ViagemAerea>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                     Dados = await srv.ListarViagemAerea(ItemCriterioBusca);

                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarViagemAerea(ItemCriterioBusca);
            }
            ListaDados = new ObservableCollection<ViagemAerea>(Dados);
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            ViagemAerea ItemViagemAerea = null;
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    ItemViagemAerea = await srv.CarregarViagemAerea(((ViagemAerea)itemSelecionado.Item).Identificador);

                }
            }
            else
            {
                ItemViagemAerea = await DatabaseService.CarregarViagemAerea(((ViagemAerea)itemSelecionado.Item).Identificador);

            }
            var Pagina = new EdicaoViagemAereaPage() { BindingContext = new EdicaoViagemAereaViewModel(ItemViagemAerea, ItemViagem) };
            await PushAsync(Pagina);
        }
        private async Task Adicionar()
        {
            var itemPontoOrigem = new ViagemAereaAeroporto() { TipoPonto = (int)enumTipoParada.Origem };
            var itemPontoDestino = new ViagemAereaAeroporto() { TipoPonto = (int)enumTipoParada.Destino};

            var ItemViagemAerea = new ViagemAerea() { Avaliacoes = new MvvmHelpers.ObservableRangeCollection<AvaliacaoAerea>(), Aeroportos = new MvvmHelpers.ObservableRangeCollection<ViagemAereaAeroporto>(new[] {itemPontoOrigem, itemPontoDestino }), DataPrevista = DateTime.Today  } ;
           
            var Pagina = new EdicaoViagemAereaPage() { BindingContext = new EdicaoViagemAereaViewModel(ItemViagemAerea, ItemViagem) };
            await PushAsync(Pagina);
                
            
        }

    }
}
