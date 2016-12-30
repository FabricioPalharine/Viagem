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
    public class ListagemCarroCustoViewModel : BaseNavigationViewModel
    {
       
        private AluguelGasto _ItemSelecionado;
        private Carro _ItemCarro;

        public ListagemCarroCustoViewModel(Viagem pitemViagem, Carro pItemCarro)
        {
            ItemViagem = pitemViagem;
            ItemCarro = pItemCarro;
            ListaDados = new ObservableCollection<AluguelGasto>(pItemCarro.Gastos.Where(d => !d.DataExclusao.HasValue));
            PageAppearingCommand = new Command(
                                                                    () =>
                                                                   {

                                                                   },
                                                                   () => true);

            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  (obj) => { });

            DeleteCommand = new Command<AluguelGasto>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoSelecionado);
            MessagingService.Current.Unsubscribe<AluguelGasto>(MessageKeys.ManutencaoAluguelGasto);
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoIncluido);
            MessagingService.Current.Subscribe<AluguelGasto>(MessageKeys.ManutencaoAluguelGasto, (service, item) =>
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
                    ItemCarro.Gastos.Add(item);
                }
                IsBusy = false;
            });
            

            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoSelecionado, async (service, item) =>
            {
                var itemGravar = new AluguelGasto() { IdentificadorCarro = ItemCarro.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now };
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarAluguelGasto(itemGravar);
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = item;
                        MessagingService.Current.SendMessage<AluguelGasto>(MessageKeys.ManutencaoAluguelGasto, itemGravar);
                    }
                }

            });
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoIncluido,  (service, item) =>
            {
                var itemGravar = new AluguelGasto() { IdentificadorCarro = ItemCarro.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now };
               
                        itemGravar.Identificador = item.Atracoes.Select(d => d.Identificador).FirstOrDefault();
                        itemGravar.ItemGasto = item;
                        MessagingService.Current.SendMessage<AluguelGasto>(MessageKeys.ManutencaoAluguelGasto, itemGravar);
                   
                

            });
        }

        private void VerificarExclusao(AluguelGasto obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir o custo {0}?", obj.ItemGasto.Descricao),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        obj.DataExclusao = DateTime.Now;
                        var Resultado = await srv.SalvarAluguelGasto(obj);
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
           


        public ObservableCollection<AluguelGasto> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

        

        public AluguelGasto ItemSelecionado
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

        public Carro ItemCarro
        {
            get
            {
                return _ItemCarro;
            }

            set
            {
                _ItemCarro = value;
            }
        }

       
      
        private async Task Adicionar()
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(string.Empty,"Cancelar",null,
                       "Novo Custo",
                       "Custo Existente"
                      );
            if (action == "Custo Existente")
            {
                var Pagina = new ListagemSelecaoCustoPage() { BindingContext = new ListagemSelecaoCustoViewModel(ItemViagem) };
                await PushAsync(Pagina);
            }
            else if (action == "Novo Custo")
            {
                var CustoCarro = new AluguelGasto() { IdentificadorCarro = ItemCarro.Identificador, DataAtualizacao = DateTime.Now };
                var ItemGasto = new Gasto()
                {
                    ApenasBaixa = false,
                    Data = DateTime.Today,
                    Dividido = false,
                    Especie = true,
                    IdentificadorUsuario = ItemUsuarioLogado.Codigo,
                    Moeda = ItemViagem.Moeda,
                    Usuarios = new MvvmHelpers.ObservableRangeCollection<GastoDividido>(),
                    Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>(),
                    Alugueis = new MvvmHelpers.ObservableRangeCollection<AluguelGasto>(new AluguelGasto[] { CustoCarro}),
                    Compras = new MvvmHelpers.ObservableRangeCollection<GastoCompra>(),
                    Atracoes = new MvvmHelpers.ObservableRangeCollection<GastoAtracao>(),
                    Reabastecimentos = new MvvmHelpers.ObservableRangeCollection<ReabastecimentoGasto>(),
                    Refeicoes = new MvvmHelpers.ObservableRangeCollection<GastoRefeicao>(),
                    ViagenAereas = new MvvmHelpers.ObservableRangeCollection<GastoViagemAerea>()
                };
                var vm = new EdicaoGastoViewModel(ItemGasto) { VoltarPagina = true };
                var pagina = new EdicaoGastoPage() { BindingContext = vm };
                await PushAsync(pagina);
            }
        }

    }
}
