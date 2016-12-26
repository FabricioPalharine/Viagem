using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{

    public class EdicaoGastoViewModel : BaseNavigationViewModel
    {
        private Gasto _ItemGasto;
        private bool _PermiteExcluir = true;
        private bool _PagamentoMECartao = false;
        private bool _ExibeHora = false;
        private bool _VoltarPagina = false;
        private Usuario _ParticipanteSelecionado;
        public EdicaoGastoViewModel(Gasto pItemGasto)
        {
           
            ItemGasto = pItemGasto;

            Participantes = new ObservableCollection<Usuario>();
            PagamentoMECartao = !pItemGasto.Especie && pItemGasto.Moeda != (int)enumMoeda.BRL;
            ItemGasto.PropertyChanged += ItemGasto_PropertyChanged;
            
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);

            ExcluirCommand = new Command(() =>  Excluir());
            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
        }

        private void ItemGasto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Especie" || e.PropertyName == "Moeda")
                PagamentoMECartao = !ItemGasto.Especie && ItemGasto.Moeda != (int)enumMoeda.BRL;

        }

       public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }

        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        public ObservableCollection<Usuario> Participantes { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            PermiteExcluir = ItemGasto.Identificador.HasValue;
            await CarregarParticipantesViagem();
        }

        private async Task CarregarParticipantesViagem()
        {
            if (!Participantes.Any())
            {
                using (ApiService srv = new ApiService())
                {
                    Participantes.Clear();
                    var ListaUsuario = await srv.ListarParticipantesViagem();
                    foreach (var itemUsuario in ListaUsuario)
                    {
                        if (!ItemGasto.Identificador.HasValue || ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                            itemUsuario.Selecionado = true;
                        if (itemUsuario.Identificador != ItemUsuarioLogado.Codigo)
                            Participantes.Add(itemUsuario);
                    }
                }
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

   

        public bool PermiteExcluir
        {
            get
            {
                return _PermiteExcluir;
            }

            set
            {
                SetProperty(ref _PermiteExcluir, value);
            }
        }

        public bool PagamentoMECartao
        {
            get
            {
                return _PagamentoMECartao;
            }

            set
            {
                SetProperty(ref _PagamentoMECartao, value);
            }
        }

     
        public Usuario ParticipanteSelecionado
        {
            get
            {
                return _ParticipanteSelecionado;
            }

            set
            {
                _ParticipanteSelecionado = null;
                OnPropertyChanged();
            }
        }

        public bool ExibeHora
        {
            get
            {
                return _ExibeHora;
            }

            set
            {
                _ExibeHora = value;
            }
        }

        public bool VoltarPagina
        {
            get
            {
                return _VoltarPagina;
            }

            set
            {
                _VoltarPagina = value;
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                foreach (Usuario itemUsuario in Participantes)
                {
                    if (itemUsuario.Selecionado)
                    {
                        if (!ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new GastoDividido() { IdentificadorUsuario = itemUsuario.Identificador };                           
                            ItemGasto.Usuarios.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemGasto.Usuarios.Remove(ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                        }
                    }
                }
                if (ExibeHora)
                {
                    ItemGasto.Data = ItemGasto.Data.GetValueOrDefault().Date.Add(ItemGasto.Hora.GetValueOrDefault());
                }
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarGasto(ItemGasto);
                    if (Resultado.Sucesso)
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemGasto.Identificador = Resultado.IdentificadorRegistro;
                        var Jresultado = (JObject)Resultado.ItemRegistro;
                        Gasto pItemGasto = Jresultado.ToObject<Gasto>();

                        ItemGasto = pItemGasto;
                        MessagingService.Current.SendMessage<Gasto>(MessageKeys.ManutencaoGasto, ItemGasto);
                        PermiteExcluir = true;
                        if (VoltarPagina)
                        {
                            await PopAsync();
                            MessagingService.Current.SendMessage<Gasto>(MessageKeys.GastoIncluido, ItemGasto);
                        }
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

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse Gasto?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        ItemGasto.DataExclusao = DateTime.Now;
                        var Resultado = await srv.ExcluirGasto(ItemGasto.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                    }
                    MessagingService.Current.SendMessage<Gasto>(MessageKeys.ManutencaoGasto, ItemGasto);
                    await PopAsync();

                })
            });


        }

      
    }
}
