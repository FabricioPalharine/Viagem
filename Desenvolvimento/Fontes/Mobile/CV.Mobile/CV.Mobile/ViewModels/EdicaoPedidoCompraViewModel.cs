using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using FormsToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class EdicaoPedidoCompraViewModel: BaseNavigationViewModel
    {
        private ListaCompra _ItemListaCompra;
        public EdicaoPedidoCompraViewModel(ListaCompra pItemListaCompra, ObservableCollection<Usuario> pListaAmigos )
        {
            ItemListaCompra = pItemListaCompra;
            ListaAmigos = new ObservableCollection<Usuario>(pListaAmigos);

            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
        }

        public Command SalvarCommand { get; set; }

        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        public ObservableCollection<Usuario> ListaAmigos { get; set; }
        public ListaCompra ItemListaCompra
        {
            get
            {
                return _ItemListaCompra;
            }

            set
            {
                SetProperty(ref _ItemListaCompra, value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarListaCompra(ItemListaCompra);
                    if (Resultado.Sucesso)
                    {
                        base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", ItemListaCompra.Identificador.GetValueOrDefault( Resultado.IdentificadorRegistro.GetValueOrDefault()), !ItemListaCompra.Identificador.HasValue);

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemListaCompra.Identificador = Resultado.IdentificadorRegistro;
                        // ItemListaCompra = JsonConvert.DeserializeXNode < ListaCompra >()
                        MessagingService.Current.SendMessage<ListaCompra>(MessageKeys.ManutencaoPedidoCompra, ItemListaCompra);
                        await PopAsync();
                    }
                    else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Problemas Validação",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });

                    }
                }
            }
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }
    }
}
