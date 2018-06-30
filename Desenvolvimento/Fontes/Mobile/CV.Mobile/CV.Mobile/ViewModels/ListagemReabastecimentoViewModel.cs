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
    public class ListagemReabastecimentoViewModel : BaseNavigationViewModel
    {

        private Reabastecimento _ItemSelecionado;
        private Carro _ItemCarro;

        public ListagemReabastecimentoViewModel(Viagem pitemViagem, Carro pItemCarro)
        {
            ItemViagem = pitemViagem;
            ItemCarro = pItemCarro;
            ListaDados = new ObservableCollection<Reabastecimento>(pItemCarro.Reabastecimentos.Where(d => !d.DataExclusao.HasValue));
            PageAppearingCommand = new Command(
                                                                    () =>
                                                                   {

                                                                   },
                                                                   () => true);

            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));

            DeleteCommand = new Command<Reabastecimento>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<Reabastecimento>(MessageKeys.ManutencaoReabastecimento);
            MessagingService.Current.Subscribe<Reabastecimento>(MessageKeys.ManutencaoReabastecimento, (service, item) =>
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
                    ItemCarro.Reabastecimentos.Add(item);
                }
                IsBusy = false;
            });


        }

        private void VerificarExclusao(Reabastecimento obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir o Reabastecimento selecionado?"),
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
                                Resultado = await srv.ExcluirReabastecimento(obj.Identificador);
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "CR", obj.Identificador.GetValueOrDefault(), false);

                                await DatabaseService.ExcluirReabastecimento(obj.Identificador, true);

                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        await DatabaseService.ExcluirReabastecimento(obj.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Reabastecimento excluído com sucesso " } };
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

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            bool Executado = false;
            Reabastecimento ItemReabastecimento = null;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ItemReabastecimento = await srv.CarregarReabastecimento(((Reabastecimento)itemSelecionado.Item).Identificador);
                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
            {
                ItemReabastecimento = await DatabaseService.Database.RetornarReabastecimento(((Reabastecimento)itemSelecionado.Item).Identificador);

            }
            var Pagina = new EdicaoReabastecimentoPage() { BindingContext = new EdicaoReabastecimentoViewModel(ItemReabastecimento) };
            await PushAsync(Pagina);

        }

        public Viagem ItemViagem { get; set; }



        public ObservableCollection<Reabastecimento> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }



        public Reabastecimento ItemSelecionado
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
                Alugueis = new MvvmHelpers.ObservableRangeCollection<AluguelGasto>(),
                Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>(),
                Compras = new MvvmHelpers.ObservableRangeCollection<GastoCompra>(),
                Atracoes = new MvvmHelpers.ObservableRangeCollection<GastoAtracao>(),
                Reabastecimentos = new MvvmHelpers.ObservableRangeCollection<ReabastecimentoGasto>(),
                Refeicoes = new MvvmHelpers.ObservableRangeCollection<GastoRefeicao>(),
                ViagenAereas = new MvvmHelpers.ObservableRangeCollection<GastoViagemAerea>()
            };

            var ItemReabastecimento = new Reabastecimento() { IdentificadorCarro = ItemCarro.Identificador, Litro = ItemViagem.UnidadeMetrica, QuantidadeReabastecida = 0, Gastos = new MvvmHelpers.ObservableRangeCollection<ReabastecimentoGasto>(new ReabastecimentoGasto[] { new ReabastecimentoGasto() { DataAtualizacao = DateTime.Now.ToUniversalTime(), ItemGasto = ItemGasto } }) };
            var Pagina = new EdicaoReabastecimentoPage() { BindingContext = new EdicaoReabastecimentoViewModel(ItemReabastecimento) };
            await PushAsync(Pagina);


        }

    }
}
