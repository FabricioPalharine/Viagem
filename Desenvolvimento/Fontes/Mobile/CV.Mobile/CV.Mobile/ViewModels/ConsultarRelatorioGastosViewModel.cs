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
    public class ConsultarRelatorioGastosViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _IsLoadingLista;
        private RelatorioGastos _ItemSelecionado;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        private LojaItens _ItemFilhoSelecionado;
        public ConsultarRelatorioGastosViewModel()
        {
            ItemCriterioBusca = new CriterioBusca() { DataInicioDe = ItemViagemSelecionada.DataInicio };
           
            PesquisarCommand = new Command(
                                                                    async () => { await CarregarListaDados();
                                                                        var Pagina = new ConsultarRelatorioGastoListaPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                        },
                                                                    () => true);
           
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ListaTipo = new ObservableCollection<ItemLista>();
            ListaTipo.Add(new ItemLista() { Codigo = "", Descricao = "Todos" });
            ListaTipo.Add(new ItemLista() { Codigo = "A", Descricao = "Atração" });
            ListaTipo.Add(new ItemLista() { Codigo = "C", Descricao = "Carro" });
            ListaTipo.Add(new ItemLista() { Codigo = "L", Descricao = "Compras" });
            ListaTipo.Add(new ItemLista() { Codigo = "VA", Descricao = "Transportes" });
            ListaTipo.Add(new ItemLista() { Codigo = "H", Descricao = "Hospedagem" });
            ListaTipo.Add(new ItemLista() { Codigo = "CR", Descricao = "Reabastecimentos" });
            ListaTipo.Add(new ItemLista() { Codigo = "R", Descricao = "Restaurantes" });


         
        }

        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                async () =>
                                {
                                    await CarregarListaUsuarios();
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
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.ListarParticipantesViagem());
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


        public ObservableCollection<RelatorioGastos> ListaDados { get; set; }
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

        public RelatorioGastos ItemSelecionado
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

        public LojaItens ItemFilhoSelecionado
        {
            get
            {
                return _ItemFilhoSelecionado;
            }

            set
            {
                _ItemFilhoSelecionado = null;
                OnPropertyChanged();
            }
        }





        private async Task CarregarListaDados()
        {
            List<RelatorioGastos> Dados = new List<RelatorioGastos>();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarRelatorioGastos(ItemCriterioBusca);
                }
            }
            catch
            {
                ApiService.ExibirMensagemErro();
            }
            ListaDados = new ObservableCollection<RelatorioGastos>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }

  
    }
}
