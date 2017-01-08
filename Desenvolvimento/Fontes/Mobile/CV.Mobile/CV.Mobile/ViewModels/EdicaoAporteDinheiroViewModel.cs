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
    public class EdicaoAporteDinheiroViewModel : BaseNavigationViewModel
    {
        private AporteDinheiro _ItemAporteDinheiro;
        private Gasto _ItemGasto;
        private bool _BaixarMoeda = false;
        public EdicaoAporteDinheiroViewModel(AporteDinheiro pItemAporteDinheiro)
        {
            ItemAporteDinheiro = pItemAporteDinheiro;
            if (ItemAporteDinheiro.ItemGasto != null)
            {
                BaixarMoeda = true;
                _ItemGasto = ItemAporteDinheiro.ItemGasto;
            }

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
            PageAppearingCommand = new Command(
                                                                   () =>
                                                                  {
                                                                      if (ItemAporteDinheiro.ItemGasto != null)
                                                                      {
                                                                          ItemGasto = ItemAporteDinheiro.ItemGasto;
                                                                      }

                                                                  },
                                                                  () => true);
        }

        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        public ObservableCollection<Usuario> ListaAmigos { get; set; }
        public AporteDinheiro ItemAporteDinheiro
        {
            get
            {
                return _ItemAporteDinheiro;
            }

            set
            {
                SetProperty(ref _ItemAporteDinheiro, value);
            }
        }

        public bool BaixarMoeda
        {
            get
            {
                return _BaixarMoeda;
            }

            set
            {
                SetProperty(ref _BaixarMoeda, value);
                if (value)
                {
                    ItemGasto = new Gasto() { Especie = true, ApenasBaixa = true, Dividido = false, Descricao = "Baixa Moeda" };
                }
                else
                    ItemGasto = null;
            }
        }

        public Gasto ItemGasto
        {
            get
            {
                return _ItemGasto;
            }

            set
            {
                SetProperty(ref _ItemGasto, value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                ItemAporteDinheiro.ItemGasto = _ItemGasto;
                ResultadoOperacao Resultado = new ResultadoOperacao();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {

                        Resultado = await srv.SalvarAporteDinheiro(ItemAporteDinheiro);
                        if (Resultado.Sucesso)
                        {
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "AD", ItemAporteDinheiro.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()), !ItemAporteDinheiro.Identificador.HasValue);
                            var itemAporte = await srv.CarregarAporteDinheiro(ItemAporteDinheiro.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                            var itemLocal = await DatabaseService.Database.RetornarAporteDinheiro(itemAporte.Identificador);
                            if (itemLocal != null)
                                itemAporte.Id = itemLocal.Id;
                            if (itemAporte.ItemGasto != null)
                            {
                                var itemGL = await DatabaseService.Database.RetornarGasto(itemAporte.ItemGasto.Identificador);
                                if (itemGL != null)
                                    itemAporte.ItemGasto.Id = itemGL.Id;
                            }
                            await DatabaseService.GravarDadosAporte(itemAporte);
                        }
                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarAporteDinheiro(ItemAporteDinheiro);
                }
                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemAporteDinheiro.Identificador = Resultado.IdentificadorRegistro;
                    // ItemAporteDinheiro = JsonConvert.DeserializeXNode < AporteDinheiro >()
                    MessagingService.Current.SendMessage<AporteDinheiro>(MessageKeys.ManutencaoAporteDinheiro, ItemAporteDinheiro);
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
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }
    }
}
