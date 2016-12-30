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
    public class ListagemCarroDeslocamentoViewModel : BaseNavigationViewModel
    {

        private CarroDeslocamento _ItemSelecionado;
        private Carro _ItemCarro;

        public ListagemCarroDeslocamentoViewModel(Viagem pitemViagem, Carro pItemCarro)
        {
            ItemViagem = pitemViagem;
            ItemCarro = pItemCarro;
            ListaDados = new ObservableCollection<CarroDeslocamento>(pItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue));
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

            DeleteCommand = new Command<CarroDeslocamento>(
                                                                   (obj) => VerificarExclusao(obj));
            MessagingService.Current.Unsubscribe<CarroDeslocamento>(MessageKeys.ManutencaoCarroDeslocamento);
            MessagingService.Current.Subscribe<CarroDeslocamento>(MessageKeys.ManutencaoCarroDeslocamento, (service, item) =>
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
                    ItemCarro.Deslocamentos.Add(item);
                }
                IsBusy = false;
            });


        }

        private void VerificarExclusao(CarroDeslocamento obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir o CarroDeslocamento selecionado?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        obj.DataExclusao = DateTime.Now;
                        var Resultado = await srv.SalvarCarroDeslocamento(obj);
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

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {

            var ItemCarroDeslocamento = ((CarroDeslocamento)itemSelecionado.Item).Clone();
            var Pagina = new EdicaoCarroDeslocamentoPage() { BindingContext = new EdicaoCarroDeslocamentoViewModel(ItemCarroDeslocamento) };
            await PushAsync(Pagina);

        }

        public Viagem ItemViagem { get; set; }



        public ObservableCollection<CarroDeslocamento> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }



        public CarroDeslocamento ItemSelecionado
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
            if (ItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => !d.ItemCarroEventoChegada.Data.HasValue).Any())
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Aviso",
                    Message = "Não pode ser Iniciado um novo deslocamento pois o anterior não foi concluído",
                    Cancel = "OK"
                });
            }
            else
            {
                int? UltimoOdometro = ItemCarro.ItemCarroEventoRetirada.Odometro;
                if (ItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarroEventoChegada.Data.HasValue).Any())
                    UltimoOdometro = ItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarroEventoChegada.Data.HasValue).Where(d => d.ItemCarroEventoChegada.Odometro.GetValueOrDefault(0) > 0).Max(d => d.ItemCarroEventoChegada.Odometro);

                var Usuarios = new MvvmHelpers.ObservableRangeCollection<CarroDeslocamentoUsuario>(ItemCarro.Avaliacoes.Where(d => !d.DataExclusao.HasValue).Select(d => new CarroDeslocamentoUsuario() { IdentificadorUsuario = d.IdentificadorUsuario }));
                var ItemCarroDeslocamento = new CarroDeslocamento() { IdentificadorCarro = ItemCarro.Identificador, Usuarios = Usuarios, ItemCarroEventoChegada = new CarroEvento() { Inicio = false }, ItemCarroEventoPartida = new CarroEvento() { Inicio = true, Odometro = UltimoOdometro, Data = DateTime.Now, Hora = DateTime.Now.TimeOfDay } };
                var Pagina = new EdicaoCarroDeslocamentoPage() { BindingContext = new EdicaoCarroDeslocamentoViewModel(ItemCarroDeslocamento) };
                await PushAsync(Pagina);
            }

        }

    }
}
