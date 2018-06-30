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
    public class ListagemHotelCustoViewModel : BaseNavigationViewModel
    {

        private GastoHotel _ItemSelecionado;
        private Hotel _ItemHotel;

        public ListagemHotelCustoViewModel(Viagem pitemViagem, Hotel pItemHotel)
        {
            ItemViagem = pitemViagem;
            ItemHotel = pItemHotel;
            ListaDados = new ObservableCollection<GastoHotel>(pItemHotel.Gastos.Where(d => !d.DataExclusao.HasValue));
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

            DeleteCommand = new Command<GastoHotel>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoSelecionado);
            MessagingService.Current.Unsubscribe<GastoHotel>(MessageKeys.ManutencaoGastoHotel);
            MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.GastoIncluido);
            MessagingService.Current.Subscribe<GastoHotel>(MessageKeys.ManutencaoGastoHotel, (service, item) =>
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
                    ItemHotel.Gastos.Add(item);
                }
                IsBusy = false;
            });


            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoSelecionado, async (service, item) =>
            {
                var itemGravar = new GastoHotel() { IdentificadorHotel = ItemHotel.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };
                bool Executado = false;
                if (Conectado)
                {
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            var Resultado = await srv.SalvarGastoHotel(itemGravar);
                            if (Resultado.Sucesso)
                            {
                                itemGravar.Identificador = Resultado.IdentificadorRegistro;
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "GH", itemGravar.Identificador.GetValueOrDefault(), true);
                                itemGravar.ItemGasto = item;
                                await DatabaseService.Database.SalvarGastoHotel(itemGravar);
                                MessagingService.Current.SendMessage<GastoHotel>(MessageKeys.ManutencaoGastoHotel, itemGravar);

                            }
                            Executado = true;
                        }
                    }
                    catch { Executado = false; }
                }
                if (!Executado)
                {
                    if (!(await DatabaseService.Database.ListarGastoHotel_IdentificadorGasto(item.Identificador)).Where(d => !d.DataExclusao.HasValue).Any())
                    {
                        itemGravar.AtualizadoBanco = false;
                        itemGravar.ItemGasto = item;

                        await DatabaseService.Database.SalvarGastoHotel(itemGravar);
                        MessagingService.Current.SendMessage<GastoHotel>(MessageKeys.ManutencaoGastoHotel, itemGravar);

                    }
                }




            });
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.GastoIncluido, (service, item) =>
           {
               var itemGravar = new GastoHotel() { IdentificadorHotel = ItemHotel.Identificador, IdentificadorGasto = item.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };

               itemGravar.Identificador = item.Atracoes.Select(d => d.Identificador).FirstOrDefault();
               itemGravar.ItemGasto = item;
               MessagingService.Current.SendMessage<GastoHotel>(MessageKeys.ManutencaoGastoHotel, itemGravar);



           });
        }

        private void VerificarExclusao(GastoHotel obj)
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
                    bool Executado = false;
                    if (Conectado)
                    {
                        try
                        {
                            using (ApiService srv = new ApiService())
                            {
                                Resultado = await srv.SalvarGastoHotel(obj);
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "GH", obj.Identificador.GetValueOrDefault(), false);

                                var itemBase = await DatabaseService.Database.RetornarGastoHotel(obj.Identificador);
                                if (itemBase != null)
                                    await DatabaseService.Database.ExcluirGastoHotel(itemBase);

                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await DatabaseService.Database.SalvarGastoHotel(obj);
                        }
                        else
                            await DatabaseService.Database.ExcluirGastoHotel(obj);
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



        public ObservableCollection<GastoHotel> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }



        public GastoHotel ItemSelecionado
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

        public Hotel ItemHotel
        {
            get
            {
                return _ItemHotel;
            }

            set
            {
                _ItemHotel = value;
            }
        }



        private async Task Adicionar()
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(string.Empty, "Cancelar", null,
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
                var CustoHotel = new GastoHotel() { IdentificadorHotel = ItemHotel.Identificador, DataAtualizacao = DateTime.Now.ToUniversalTime() };
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
                    Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>(new GastoHotel[] { CustoHotel }),
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
