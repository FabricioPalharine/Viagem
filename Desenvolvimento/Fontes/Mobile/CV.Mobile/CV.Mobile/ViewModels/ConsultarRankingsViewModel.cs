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
    public class ConsultarRankingsViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        public bool ExibeViagem { get; set; }
        private bool _IsLoadingLista;
        private object _ItemSelecionado;
        private bool _ExibeAmigo;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        public ConsultarRankingsViewModel()
        {
            ExibeViagem = ItemViagemSelecionada != null;
            ItemCriterioBusca = new CriterioBusca() { Aberto = false, TipoInteiro = 1, Count = 20 };
            ItemCriterioBusca.PropertyChanged += ItemCriterioBusca_PropertyChanged;
            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaDados();
                                                                        var Pagina = new ConsultarRankingsListaPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                    },
                                                                    () => true);

            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ListaTipoAmigo = new ObservableCollection<ItemLista>();
            ListaTipoAmigo.Add(new ItemLista() { Codigo = "1", Descricao = "Todos" });
            ListaTipoAmigo.Add(new ItemLista() { Codigo = "2", Descricao = "Amigos" });
            ListaTipoAmigo.Add(new ItemLista() { Codigo = "3", Descricao = "Um Amigo" });

            ListaTipo = new ObservableCollection<ItemLista>();
            ListaTipo.Add(new ItemLista() { Codigo = "", Descricao = "Todos" });
            ListaTipo.Add(new ItemLista() { Codigo = "A", Descricao = "Atração" });
            ListaTipo.Add(new ItemLista() { Codigo = "C", Descricao = "Locadoras" });
            ListaTipo.Add(new ItemLista() { Codigo = "L", Descricao = "Lojas" });
            ListaTipo.Add(new ItemLista() { Codigo = "VA", Descricao = "Companhias Viagem" });
            ListaTipo.Add(new ItemLista() { Codigo = "H", Descricao = "Hospedagem" });
            ListaTipo.Add(new ItemLista() { Codigo = "R", Descricao = "Restaurantes" });
            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                          async (obj) => await VerificarAcaoItem(obj));

        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            ConsultaRankings ItemRanking = (ConsultaRankings)itemSelecionado.Item;
            CriterioBusca itemConsulta = ItemCriterioBusca.Clone();
            itemConsulta.Tipo = ItemRanking.Tipo;
            itemConsulta.Comentario = ItemRanking.CodigoGoogle;
            itemConsulta.Nome = ItemRanking.Nome;
            try
            {
                using (ApiService srv = new ApiService())
                {
                    ListaAvaliacoes = new ObservableCollection<UsuarioConsulta>(await srv.ListarAvaliacoesRankings(itemConsulta));
                    OnPropertyChanged("ListaAvaliacoes");
                }




                var Pagina = new ConsultarRankingsDetalhePage() { BindingContext = this };
                await PushAsync(Pagina);
            }
            catch
            {
                ApiService.ExibirMensagemErro();
            }

        }

        private void ItemCriterioBusca_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TipoInteiro")
                ExibeAmigo = ItemCriterioBusca.TipoInteiro == 3;
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
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.ListarAmigosComigo());
                    OnPropertyChanged("ListaUsuario");
                }
            }
        }

        public ObservableCollection<ItemLista> ListaTipo { get; set; }
        public ObservableCollection<ItemLista> ListaTipoAmigo { get; set; }
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


        public ObservableCollection<ConsultaRankings> ListaDados { get; set; }
        public ObservableCollection<UsuarioConsulta> ListaAvaliacoes { get; set; }

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

        public object ItemSelecionado
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



        public bool ExibeAmigo
        {
            get
            {
                return _ExibeAmigo;
            }

            set
            {
                SetProperty(ref _ExibeAmigo, value);
            }
        }

        private async Task CarregarListaDados()
        {
            List<ConsultaRankings> Dados = new List<ConsultaRankings>();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarRankings(ItemCriterioBusca);
                }

                ListaDados = new ObservableCollection<ConsultaRankings>(Dados);
                OnPropertyChanged("ListaDados");

            }
            catch
            {
                ApiService.ExibirMensagemErro();
            }
            IsLoadingLista = false;
        }


    }
}
