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
    public class EdicaoItemCompraViewModel : BaseNavigationViewModel
    {
        private ItemCompra _ItemItemCompra;
        private bool _CadastradoComoAmigo;
        private bool _PermiteExcluir = true;
        private bool _Loading = true;
        public EdicaoItemCompraViewModel(ItemCompra pItemItemCompra )
        {
            ItemItemCompra = pItemItemCompra.Clone();
            CadastradoComoAmigo = ItemItemCompra.IdentificadorUsuario.HasValue;
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
            DeleteCommand = new Command(
                               () =>  VerificarExclusao(),
                              () => true);

            PageAppearingCommand = new Command(
                                async () => await CarregarPagina(),
                                () => true);
            SelecaoItemAlteradaCommand = new Command<EventArgs>((obj) => TrocarItemSelecao(obj));
        }

      

        public Command SalvarCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command PageAppearingCommand { get; set; }

        public Command<EventArgs> SelecaoItemAlteradaCommand { get; set; }
        public ObservableCollection<Usuario> ListaAmigos { get; set; }

        public ObservableCollection<ListaCompra> ListaListaCompra { get; set; }

        public ItemCompra ItemItemCompra
        {
            get
            {
                return _ItemItemCompra;
            }

            set
            {
                SetProperty(ref _ItemItemCompra, value);
            }
        }

        public bool CadastradoComoAmigo
        {
            get
            {
                return _CadastradoComoAmigo;
            }

            set
            {
                SetProperty(ref _CadastradoComoAmigo, value);
                if (value)
                    ItemItemCompra.Destinatario = null;
                else
                    ItemItemCompra.IdentificadorUsuario = null;
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

        private async void TrocarItemSelecao(EventArgs obj)
        {
            if (!_Loading)
            {
                await Task.Delay(200);
                ListaCompra itemLista = ListaListaCompra.Where(d => d.Identificador == ItemItemCompra.IdentificadorListaCompra).FirstOrDefault();
                if (itemLista != null)
                {
                    ItemItemCompra.Marca = itemLista.Marca;
                    ItemItemCompra.Descricao = itemLista.Descricao;
                    if (itemLista.IdentificadorUsuario == ItemUsuarioLogado.Codigo)
                    {
                        CadastradoComoAmigo = itemLista.IdentificadorUsuarioPedido.HasValue;
                        ItemItemCompra.IdentificadorUsuario = itemLista.IdentificadorUsuarioPedido;
                        ItemItemCompra.Destinatario = itemLista.Destinatario;
                    }
                    else
                    {
                        CadastradoComoAmigo = true;
                        ItemItemCompra.IdentificadorUsuario = itemLista.IdentificadorUsuario;
                    }
                    ItemItemCompra.Reembolsavel = itemLista.Reembolsavel;
                }
            }
        }

        private void VerificarExclusao()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse item"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        ItemItemCompra.DataExclusao = DateTime.Now;
                        var Resultado = await srv.SalvarItemCompra(ItemItemCompra);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        MessagingService.Current.SendMessage<ItemCompra>(MessageKeys.ManutencaoItemCompra, ItemItemCompra);
                        await PopAsync();
                    }


                })
            });
        }

        private async Task CarregarPagina()
        {
            await Task.Delay(100);
            PermiteExcluir = ItemItemCompra.Identificador.HasValue;
            CarregarParticipantesViagem();
            CarregarListaCompra();

        }

        private async void CarregarParticipantesViagem()
        {

            using (ApiService srv = new ApiService())
            {

                var ListaUsuario = await srv.ListarAmigos();
                ListaAmigos = new ObservableCollection<Usuario>(ListaUsuario);
                OnPropertyChanged("ListaAmigos");
                int? IdentificadorRefeicaoSelecionada = ItemItemCompra.IdentificadorUsuario;
                ItemItemCompra.IdentificadorUsuario = null;
                await Task.Delay(100);
                ItemItemCompra.IdentificadorUsuario = IdentificadorRefeicaoSelecionada;

            }

        }

        public async void CarregarListaCompra()
        {
            using (ApiService srv = new ApiService())
            {
                var ListaDados = await srv.CarregarListaPedidos(new CriterioBusca() { IdentificadorParticipante = ItemUsuarioLogado.Codigo });
                ListaListaCompra = new ObservableCollection<ListaCompra>(ListaDados);
                OnPropertyChanged("ListaListaCompra");
                int? IdentificadorRefeicaoSelecionada = ItemItemCompra.IdentificadorListaCompra;
                ItemItemCompra.IdentificadorListaCompra = null;
                await Task.Delay(100);
                ItemItemCompra.IdentificadorListaCompra = IdentificadorRefeicaoSelecionada;
                await Task.Delay(100);
                _Loading = false;
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
                    var Resultado = await srv.SalvarItemCompra(ItemItemCompra);
                    if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemItemCompra.Identificador = Resultado.IdentificadorRegistro;
                        MessagingService.Current.SendMessage<ItemCompra>(MessageKeys.ManutencaoItemCompra, ItemItemCompra);
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
