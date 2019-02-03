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
    public class EdicaoRequisicaoCompraViewModel : BaseNavigationViewModel
    {
        private ListaCompra _ItemListaCompra;
        private bool _PermiteCancelar = true;
        public EdicaoRequisicaoCompraViewModel(ListaCompra pItemListaCompra )
        {
            ItemListaCompra = pItemListaCompra;
            CancelarCommand = new Command(
                                 () =>  Cancelar(),
                                () => true);
            PageAppearingCommand = new Command(async () => await CarregarPagina(), () => true);
        }

        public Command CancelarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }

        public async Task CarregarPagina()
        {
            await Task.Delay(100);
            PermiteCancelar = ItemListaCompra.Status == (int)enumStatusListaCompra.Pendente || ItemListaCompra.Status == (int)enumStatusListaCompra.NaoVisto;
        }
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

        public bool PermiteCancelar
        {
            get
            {
                return _PermiteCancelar;
            }

            set
            {
                SetProperty(ref _PermiteCancelar, value); 
            }
        }

        private  void Cancelar()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Ao ignorar esse pedido não será possível associar uma compra a ele, deseja prosseguir?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemListaCompra.Status = (int)enumStatusListaCompra.NaoComprar;
                    ItemListaCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try { 
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await  srv.SalvarListaCompra(ItemListaCompra);
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", ItemListaCompra.Identificador.GetValueOrDefault(), false);

                            var itemBanco = await DatabaseService.Database.RetornarListaCompra(ItemListaCompra.Identificador);
                            if (itemBanco != null)
                            {
                                ItemListaCompra.Id = itemBanco.Id;

                            }
                            await DatabaseService.Database.SalvarListaCompra(ItemListaCompra);
                        }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                          ItemListaCompra.AtualizadoBanco = false;
                           await DatabaseService.Database.SalvarListaCompra(ItemListaCompra);
                       

                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Lista Compra Salva com Sucesso" } };
                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<ListaCompra>(MessageKeys.ManutencaoRequisicaoPedidoCompra, ItemListaCompra);
                    await PopAsync();

                })
            });

           
        }
    }
}
