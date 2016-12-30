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
    public class ListagemViagemAereaCustoViewModel : BaseNavigationViewModel
    {
       
        private GastoViagemAerea _ItemSelecionado;
        private ViagemAerea _ItemViagemAerea;

        public ListagemViagemAereaCustoViewModel(Viagem pitemViagem, ViagemAerea pItemViagemAerea)
        {
            ItemViagem = pitemViagem;
            ItemViagemAerea = pItemViagemAerea;
            ListaDados = new ObservableCollection<GastoViagemAerea>(pItemViagemAerea.Gastos.Where(d => !d.DataExclusao.HasValue));
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

            DeleteCommand = new Command<GastoViagemAerea>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoSelecionado);
            MessagingService.Current.Unsubscribe<GastoViagemAerea>(MessageKeys.ManutencaoGastoViagemAerea);
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoIncluido);
            MessagingService.Current.Subscribe<GastoViagemAerea>(MessageKeys.ManutencaoGastoViagemAerea, (service, item) =>
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
                    ItemViagemAerea.Gastos.Add(item);
                }
                IsBusy = false;
            });
            

            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoSelecionado, async (service, item) =>
            {
                var itemGravar = new GastoViagemAerea() { IdentificadorViagemAerea = ItemViagemAerea.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now };
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarGastoViagemAerea(itemGravar);
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = item;
                        MessagingService.Current.SendMessage<GastoViagemAerea>(MessageKeys.ManutencaoGastoViagemAerea, itemGravar);
                    }
                }

            });
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoIncluido,  (service, item) =>
            {
                var itemGravar = new GastoViagemAerea() { IdentificadorViagemAerea = ItemViagemAerea.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now };
               
                        itemGravar.Identificador = item.Atracoes.Select(d => d.Identificador).FirstOrDefault();
                        itemGravar.ItemGasto = item;
                        MessagingService.Current.SendMessage<GastoViagemAerea>(MessageKeys.ManutencaoGastoViagemAerea, itemGravar);
                   
                

            });
        }

        private void VerificarExclusao(GastoViagemAerea obj)
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
                        var Resultado = await srv.SalvarGastoViagemAerea(obj);
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
           


        public ObservableCollection<GastoViagemAerea> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

        

        public GastoViagemAerea ItemSelecionado
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

        public ViagemAerea ItemViagemAerea
        {
            get
            {
                return _ItemViagemAerea;
            }

            set
            {
                _ItemViagemAerea = value;
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
                var CustoViagemAerea = new GastoViagemAerea() { IdentificadorViagemAerea = ItemViagemAerea.Identificador, DataAtualizacao = DateTime.Now };
                var ItemGasto = new Gasto()
                {
                    ApenasBaixa = false,
                    Data = DateTime.Today,
                    Dividido = false,
                    Especie = true,
                    IdentificadorUsuario = ItemUsuarioLogado.Codigo,
                    Moeda = ItemViagem.Moeda,
                    Usuarios = new MvvmHelpers.ObservableRangeCollection<GastoDividido>(),
                    Alugueis = new MvvmHelpers.ObservableRangeCollection<AluguelGasto>(),
                    ViagenAereas = new MvvmHelpers.ObservableRangeCollection<GastoViagemAerea>(new GastoViagemAerea[] { CustoViagemAerea}),
                    Compras = new MvvmHelpers.ObservableRangeCollection<GastoCompra>(),
                    Atracoes = new MvvmHelpers.ObservableRangeCollection<GastoAtracao>(),
                    Reabastecimentos = new MvvmHelpers.ObservableRangeCollection<ReabastecimentoGasto>(),
                    Refeicoes = new MvvmHelpers.ObservableRangeCollection<GastoRefeicao>(),
                    Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>()
                };
                var vm = new EdicaoGastoViewModel(ItemGasto) { VoltarPagina = true };
                var pagina = new EdicaoGastoPage() { BindingContext = vm };
                await PushAsync(pagina);
            }
        }

    }
}
