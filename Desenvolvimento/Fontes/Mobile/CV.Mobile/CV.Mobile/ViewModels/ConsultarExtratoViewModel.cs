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
    public class ConsultarExtratoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _IsLoadingLista;
        private ExtratoMoeda _ItemSelecionado;


        public ConsultarExtratoViewModel()
        {
            ItemCriterioBusca = new CriterioBusca() { DataInicioDe = ItemViagemSelecionada.DataInicio, Moeda = ItemViagemSelecionada.Moeda };

            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaDados();
                                                                        var Pagina = new ConsultarExtratoListaPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                    },
                                                                    () => true);

            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
            OnPropertyChanged("ListaMoeda");
        }

        public ObservableCollection<ItemLista> ListaMoeda { get; set; }
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


        public ObservableCollection<ExtratoMoeda> ListaDados { get; set; }
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

        public ExtratoMoeda ItemSelecionado
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
            List<ExtratoMoeda> Dados = new List<ExtratoMoeda>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarExtratoMoeda(ItemCriterioBusca);
                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
                Dados = await DatabaseService.Database.ConsultarExtratoMoeda(ItemUsuarioLogado.Codigo, ItemViagemSelecionada.Identificador, ItemCriterioBusca.Moeda, ItemCriterioBusca.DataInicioDe.GetValueOrDefault());
            ListaDados = new ObservableCollection<ExtratoMoeda>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }


    }
}
