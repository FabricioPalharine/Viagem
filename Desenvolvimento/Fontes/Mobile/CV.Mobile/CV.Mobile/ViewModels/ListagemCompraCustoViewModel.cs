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
    public class ListagemCompraCustoViewModel : BaseNavigationViewModel
    {

        private GastoCompra _ItemSelecionado;
        private Loja _ItemLoja;


        public ListagemCompraCustoViewModel(Viagem pitemViagem, Loja pItemLoja)
        {
            ItemViagem = pitemViagem;
            ItemLoja = pItemLoja;
            ListaDados = new ObservableCollection<GastoCompra>(pItemLoja.Compras.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemGasto.IdentificadorUsuario == ItemUsuarioLogado.Codigo));
            PageAppearingCommand = new Command(
                                                                    () =>
                                                                   {

                                                                   },
                                                                   () => true);

            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => { await VerificarAcaoItem(obj); });

            DeleteCommand = new Command<GastoCompra>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<GastoCompra>(MessageKeys.ManutencaoGastoCompra);
            MessagingService.Current.Subscribe<GastoCompra>(MessageKeys.ManutencaoGastoCompra, (service, item) =>
            {
                IsBusy = true;

                if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                {
                    var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                    ListaDados.RemoveAt(Posicao);
                    if (!item.DataExclusao.HasValue)
                    {
                        ListaDados.Insert(Posicao, item);
                    }
                }
                else if (!item.DataExclusao.HasValue)
                {
                    ListaDados.Add(item);
                    ItemLoja.Compras.Add(item);
                }
                IsBusy = false;
            });

        }

        private void VerificarExclusao(GastoCompra obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir a compra {0}?", obj.ItemGasto.Descricao),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        obj.DataExclusao = DateTime.Now;
                        var Resultado = await srv.ExcluirGastoCompra(obj);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ListaDados.Remove(obj);
                    }


                })
            });
        }

        public Viagem ItemViagem { get; set; }



        public ObservableCollection<GastoCompra> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }



        public GastoCompra ItemSelecionado
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

        public Loja ItemLoja
        {
            get
            {
                return _ItemLoja;
            }

            set
            {
                _ItemLoja = value;
            }
        }


        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            using (ApiService srv = new ApiService())
            {
                var Item = ((GastoCompra)itemSelecionado.Item);
                var Pagina = new EdicaoCompraPage() { BindingContext = new EdicaoCompraViewModel(Item) };
                await PushAsync(Pagina);
            }
        }

        private async Task Adicionar()
        {

            var CustoLoja = new GastoCompra() { IdentificadorLoja = ItemLoja.Identificador, DataAtualizacao = DateTime.Now };
            var ItemGasto = new Gasto()
            {
                ApenasBaixa = false,
                Data = DateTime.Today,
                Hora = DateTime.Now.TimeOfDay,
                Dividido = false,
                Especie = true,
                IdentificadorUsuario = ItemUsuarioLogado.Codigo,
                Moeda = ItemViagem.Moeda,
                Usuarios = new MvvmHelpers.ObservableRangeCollection<GastoDividido>(),
                IdentificadorViagem = ItemViagem.Identificador

            };
            CustoLoja.ItemGasto = ItemGasto;
            CustoLoja.ItensComprados = new MvvmHelpers.ObservableRangeCollection<ItemCompra>();
            var vm = new EdicaoCompraViewModel(CustoLoja) { VoltarPagina = true };
            var pagina = new EdicaoCompraPage() { BindingContext = vm };
            await PushAsync(pagina);
        }
    }

}

