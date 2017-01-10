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
    public class ListagemAtracaoCustoViewModel : BaseNavigationViewModel
    {
       
        private GastoAtracao _ItemSelecionado;
        private Atracao _ItemAtracao;

        public ListagemAtracaoCustoViewModel(Viagem pitemViagem, Atracao pItemAtracao)
        {
            ItemViagem = pitemViagem;
            ItemAtracao = pItemAtracao;
            ListaDados = new ObservableCollection<GastoAtracao>(pItemAtracao.Gastos.Where(d => !d.DataExclusao.HasValue));
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

            DeleteCommand = new Command<GastoAtracao>(
                                                                   (obj) => VerificarExclusao(obj));

            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoSelecionado);
            MessagingService.Current.Unsubscribe<GastoAtracao>(MessageKeys.ManutencaoGastoAtracao);
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoIncluido);
            MessagingService.Current.Subscribe<GastoAtracao>(MessageKeys.ManutencaoGastoAtracao, (service, item) =>
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
                    ItemAtracao.Gastos.Add(item);
                }
                IsBusy = false;
            });
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoSelecionado, async (service, item) =>
            {
                var itemGravar = new GastoAtracao() { IdentificadorAtracao = ItemAtracao.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.SalvarGastoAtracao(itemGravar);
                        if (Resultado.Sucesso)
                        {
                            itemGravar.Identificador = Resultado.IdentificadorRegistro;
                            AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "GA", itemGravar.Identificador.GetValueOrDefault(), true);
                            itemGravar.ItemGasto = item;
                            await DatabaseService.Database.SalvarGastoAtracao(itemGravar);
                            MessagingService.Current.SendMessage<GastoAtracao>(MessageKeys.ManutencaoGastoAtracao, itemGravar);
                        }
                    }
                }
                else
                {
                    itemGravar.AtualizadoBanco = false;
                    await DatabaseService.Database.SalvarGastoAtracao(itemGravar);

                }

            });
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoIncluido,  (service, item) =>
            {
                var itemGravar = new GastoAtracao() { IdentificadorAtracao = ItemAtracao.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };
                itemGravar.Identificador = item.Atracoes.Select(d => d.Identificador).FirstOrDefault();
                itemGravar.ItemGasto = item;

                MessagingService.Current.SendMessage<GastoAtracao>(MessageKeys.ManutencaoGastoAtracao, itemGravar);
                   
                

            });
        }

        private void VerificarExclusao(GastoAtracao obj)
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
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    obj.DataExclusao = DateTime.Now.ToUniversalTime();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.SalvarGastoAtracao(obj);
                            var itemBase = await DatabaseService.Database.RetornarGastoAtracao(obj.Identificador);
                            if (itemBase != null)
                                await DatabaseService.Database.ExcluirGastoAtracao(itemBase);
                        }
                    }
                    else
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await DatabaseService.Database.SalvarGastoAtracao(obj);
                        }
                        else
                            await DatabaseService.Database.ExcluirGastoAtracao(obj);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Gasto excluído com sucesso " } };

                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ListaDados.Remove(obj);
                })
            });
        }

        public Viagem ItemViagem { get; set; }
           


        public ObservableCollection<GastoAtracao> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

        

        public GastoAtracao ItemSelecionado
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

        public Atracao ItemAtracao
        {
            get
            {
                return _ItemAtracao;
            }

            set
            {
                _ItemAtracao = value;
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
                var CustoAtracao = new GastoAtracao() { IdentificadorAtracao = ItemAtracao.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };
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
                    Atracoes = new MvvmHelpers.ObservableRangeCollection<GastoAtracao>(new GastoAtracao[] { CustoAtracao}),
                    Compras = new MvvmHelpers.ObservableRangeCollection<GastoCompra>(),
                    Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>(),
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
