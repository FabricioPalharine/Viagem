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
    public class ListagemItemCompraViewModel : BaseNavigationViewModel
    {
       
        private ItemCompra _ItemSelecionado;
        private GastoCompra _ItemGastoCompra;

        public ListagemItemCompraViewModel( GastoCompra pItemGastoCompra)
        {
            ItemGastoCompra = pItemGastoCompra;
            ListaDados = new ObservableCollection<ItemCompra>(pItemGastoCompra.ItensComprados.Where(d => !d.DataExclusao.HasValue));
        

            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                 async (obj) => { await VerificarAcaoItem(obj); });

            DeleteCommand = new Command<ItemCompra>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<ItemCompra>(MessageKeys.ManutencaoItemCompra);
            MessagingService.Current.Subscribe<ItemCompra>(MessageKeys.ManutencaoItemCompra, (service, item) =>
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
                    ItemGastoCompra.ItensComprados.Add(item);
                }
                IsBusy = false;
            });

        }

        private void VerificarExclusao(ItemCompra obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir o item {0} da Marca {1}?", obj.Descricao, obj.Marca),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        obj.DataExclusao = DateTime.Now;
                        var Resultado = await srv.SalvarItemCompra(obj);
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

      

        public ObservableCollection<ItemCompra> ListaDados { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

        

        public ItemCompra ItemSelecionado
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

        public GastoCompra ItemGastoCompra
        {
            get
            {
                return _ItemGastoCompra;
            }

            set
            {
                _ItemGastoCompra = value;
            }
        }


        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
         
                var Item = ((ItemCompra)itemSelecionado.Item);
                var Pagina = new EdicaoItemCompraPage() { BindingContext = new EdicaoItemCompraViewModel(Item) };
                await PushAsync(Pagina);
            
        }

        private async Task Adicionar()
        {
            var Item = new ItemCompra() { Reembolsavel=false, IdentificadorGastoCompra = ItemGastoCompra.Identificador };
            var Pagina = new EdicaoItemCompraPage() { BindingContext = new EdicaoItemCompraViewModel(Item) };
            await PushAsync(Pagina);

        }

    }
}
