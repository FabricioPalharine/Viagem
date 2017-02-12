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
    public class ConsultarLocaisVisitadosViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _IsLoadingLista;
        private object _ItemSelecionado;
        public bool VerCustos { get; set; }
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        public ObservableCollection<object> ListaDetalhes { get; set; }
        public ConsultarLocaisVisitadosViewModel()
        {
            VerCustos = ItemViagemSelecionada.VejoGastos;
            ItemCriterioBusca = new CriterioBusca() { };
            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaDados();
                                                                        var Pagina = new ConsultarLocaisVisitadosListaPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                    },
                                                                    () => true);

            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

           
            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                          async (obj) => await VerificarAcaoItem(obj));
            
            ItemTappedFilhoCommand = new Command<ItemTappedEventArgs>(
                                                          async (obj) => await VerificarAcaoItemFilho(obj));
            AbrirMapaCommand = new Command<LocalSelecionado>(async (item) => await AbrirMapa(item));
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            LocaisVisitados ItemRanking = (LocaisVisitados)itemSelecionado.Item;
            CriterioBusca itemConsulta = ItemCriterioBusca.Clone();
            itemConsulta.Tipo = ItemRanking.Tipo;
            itemConsulta.Comentario = ItemRanking.CodigoCoogle;
            itemConsulta.Nome = ItemRanking.Nome;
            LocaisVisitados itemDetalhe = null;
            using (ApiService srv = new ApiService())
            {
                if (ItemRanking.Tipo == "A")
                    itemDetalhe = await srv.ConsultarDetalheAtracao(itemConsulta);
                else if (ItemRanking.Tipo == "H")
                    itemDetalhe = await srv.ConsultarDetalheHotel(itemConsulta);
                else if(ItemRanking.Tipo == "R")
                    itemDetalhe = await srv.ConsultarDetalheRestaurante(itemConsulta);
               else if (ItemRanking.Tipo == "L")
                    itemDetalhe = await srv.ConsultarDetalheLoja(itemConsulta);
            }
            ListaDetalhes = new ObservableCollection<object>();
            itemDetalhe.Nome = ItemRanking.Nome;
            itemDetalhe.NomeCidade = ItemRanking.NomeCidade;
            itemDetalhe.Tipo = ItemRanking.Tipo;
            itemDetalhe.CodigoCoogle = ItemRanking.CodigoCoogle;
            itemDetalhe.Latitude = ItemRanking.Latitude;
            itemDetalhe.Longitude = ItemRanking.Longitude;

            ListaDetalhes.Add(new LocalSelecionado() { ItemLocal = itemDetalhe });
            if (itemDetalhe.Detalhes != null && itemDetalhe.Detalhes.Any())
            {
                ListaDetalhes.Add(new Cabecalho() { Texto = "Visitas" });
                foreach (var item in itemDetalhe.Detalhes)
                    ListaDetalhes.Add(item);
            }
            if (itemDetalhe.Gastos != null && itemDetalhe.Gastos.Any())
            {
                if (itemDetalhe.Tipo == "L")
                    ListaDetalhes.Add(new Cabecalho() { Texto = "Compras" });
                else
                    ListaDetalhes.Add(new Cabecalho() { Texto = "Gastos" });
                foreach (var item in itemDetalhe.Gastos)
                {
                    ListaDetalhes.Add(item);
                    if (item.Itens != null && item.Itens.Any())
                    {
                        ListaDetalhes.Add(new Cabecalho() { Texto = "Itens Comprados" });

                        foreach (var itemItem in item.Itens)
                        {
                            ListaDetalhes.Add(itemItem);
                        }
                    }
                }
            }
            if (itemDetalhe.Fotos != null && itemDetalhe.Fotos.Any())
                ListaDetalhes.Add(itemDetalhe.Fotos);
            if (itemDetalhe.LocaisFilho != null && itemDetalhe.LocaisFilho.Any())
            {
                ListaDetalhes.Add(new Cabecalho() { Texto = "Locais Filho" });
                foreach (var item in itemDetalhe.LocaisFilho)
                    ListaDetalhes.Add(item);
            }
            var Pagina = new ConsultarLocalVisitadoDetalhePage() { BindingContext = this };
            await PushAsync(Pagina);

        }

        private async Task VerificarAcaoItemFilho(ItemTappedEventArgs itemSelecionado)
        {
            if (itemSelecionado.Item is LocaisVisitados)
            {
                LocaisVisitados ItemRanking = ((LocaisVisitados)itemSelecionado.Item);
                CriterioBusca itemConsulta = ItemCriterioBusca.Clone();
                itemConsulta.Tipo = ItemRanking.Tipo;
                itemConsulta.Comentario = ItemRanking.CodigoCoogle;
                itemConsulta.Nome = ItemRanking.Nome;
                LocaisVisitados itemDetalhe = null;
                using (ApiService srv = new ApiService())
                {
                    if (ItemRanking.Tipo == "A")
                        itemDetalhe = await srv.ConsultarDetalheAtracao(itemConsulta);
                    else if (ItemRanking.Tipo == "H")
                        itemDetalhe = await srv.ConsultarDetalheHotel(itemConsulta);
                    else if (ItemRanking.Tipo == "R")
                        itemDetalhe = await srv.ConsultarDetalheRestaurante(itemConsulta);
                    else if (ItemRanking.Tipo == "L")
                        itemDetalhe = await srv.ConsultarDetalheLoja(itemConsulta);
                }
                itemDetalhe.Nome = ItemRanking.Nome;
                itemDetalhe.NomeCidade = ItemRanking.NomeCidade;
                itemDetalhe.Tipo = ItemRanking.Tipo;
                itemDetalhe.CodigoCoogle = ItemRanking.CodigoCoogle;
                itemDetalhe.Latitude = ItemRanking.Latitude;
                itemDetalhe.Longitude = ItemRanking.Longitude;
                var vm = new ConsultarLocaisVisitadosViewModel();
                vm.ListaDetalhes = new ObservableCollection<object>();
                vm.ListaDetalhes.Add(new LocalSelecionado() { ItemLocal = itemDetalhe });

                if (itemDetalhe.Detalhes != null && itemDetalhe.Detalhes.Any())
                {
                    vm.ListaDetalhes.Add(new Cabecalho() { Texto = "Visitas" });
                    foreach (var item in itemDetalhe.Detalhes)
                        vm.ListaDetalhes.Add(item);
                }
                if (itemDetalhe.Gastos != null && itemDetalhe.Gastos.Any())
                {
                    if (itemDetalhe.Tipo == "L")
                        vm.ListaDetalhes.Add(new Cabecalho() { Texto = "Compras" });
                    else
                        vm.ListaDetalhes.Add(new Cabecalho() { Texto = "Gastos" });
                    foreach (var item in itemDetalhe.Gastos)
                    {
                        vm.ListaDetalhes.Add(item);
                        if (item.Itens != null && item.Itens.Any())
                        {
                            vm.ListaDetalhes.Add(new Cabecalho() { Texto = "Itens Comprados" });

                            foreach (var itemItem in item.Itens)
                            {
                                vm.ListaDetalhes.Add(itemItem);
                            }
                        }
                    }
                }
                if (itemDetalhe.Fotos != null && itemDetalhe.Fotos.Any())
                    vm.ListaDetalhes.Add(itemDetalhe.Fotos);
                if (itemDetalhe.LocaisFilho != null && itemDetalhe.LocaisFilho.Any())
                {
                    vm.ListaDetalhes.Add(new Cabecalho() { Texto = "Locais Filho" });
                    foreach (var item in itemDetalhe.LocaisFilho)
                        vm.ListaDetalhes.Add(item);
                }

                var Pagina = new ConsultarLocalVisitadoDetalhePage() { BindingContext = vm };
                await PushAsync(Pagina);
            }

        }

        private async Task AbrirMapa(LocalSelecionado item)
        {
            PosicaoMapaViewModel vm = new PosicaoMapaViewModel(new Xamarin.Forms.Maps.Position(item.ItemLocal.Latitude.GetValueOrDefault(), item.ItemLocal.Longitude.GetValueOrDefault()));
            Page pagina = new PosicaoMapaPage() { BindingContext = vm };
            await PushAsync(pagina);
        }
        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                 () =>
                                {
                                    
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
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.CarregarParticipantesAmigo());
                    OnPropertyChanged("ListaUsuario");
                }
            }
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


        public ObservableCollection<LocaisVisitados> ListaDados { get; set; }

        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }
        public Command ItemTappedFilhoCommand { get; set; }

        public Command AbrirMapaCommand { get; set; }
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



      

        private async Task CarregarListaDados()
        {
            List<LocaisVisitados> Dados = new List<LocaisVisitados>();

            using (ApiService srv = new ApiService())
            {
                Dados = await srv.ListarLocaisVisitados(ItemCriterioBusca);
            }

            ListaDados = new ObservableCollection<LocaisVisitados>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }


    }
}
