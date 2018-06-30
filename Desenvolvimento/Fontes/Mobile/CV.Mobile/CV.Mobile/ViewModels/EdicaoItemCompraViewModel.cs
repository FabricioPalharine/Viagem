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
        private int? _IdentificadorListaCompraInicial = null;
        public EdicaoItemCompraViewModel(ItemCompra pItemItemCompra)
        {
            ItemItemCompra = pItemItemCompra.Clone();
            _IdentificadorListaCompraInicial = pItemItemCompra.IdentificadorListaCompra;
            CadastradoComoAmigo = ItemItemCompra.IdentificadorUsuario.HasValue;
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => !IsBusy);
            DeleteCommand = new Command(
                               () => VerificarExclusao(),
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

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemItemCompra.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try
                        {
                            using (ApiService srv = new ApiService())
                            {
                                Resultado = await srv.SalvarItemCompra(ItemItemCompra);
                                AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "IC", ItemItemCompra.Identificador.GetValueOrDefault(), false);
                                if (ItemItemCompra.IdentificadorListaCompra.HasValue)
                                {
                                    AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", ItemItemCompra.IdentificadorListaCompra.GetValueOrDefault(), false);
                                    var ItemListaCompra = await srv.CarregarListaCompra(ItemItemCompra.Identificador);
                                    var itemLCBase = await DatabaseService.Database.RetornarListaCompra(ItemListaCompra.Identificador);
                                    if (itemLCBase != null)
                                        ItemListaCompra.Id = itemLCBase.Id;
                                    await DatabaseService.Database.SalvarListaCompra(ItemListaCompra);
                                }
                                var itemBase = await DatabaseService.Database.RetornarItemCompra(ItemItemCompra.Identificador);
                                if (itemBase != null)
                                    await DatabaseService.Database.ExcluirItemCompra(itemBase);

                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        ItemItemCompra.AtualizadoBanco = false;
                        if (ItemItemCompra.IdentificadorListaCompra.HasValue)
                        {
                            var itemLCBase = await DatabaseService.Database.RetornarListaCompra(ItemItemCompra.IdentificadorListaCompra);
                            if (itemLCBase != null)
                            {
                                itemLCBase.DataAtualizacao = DateTime.Now.ToUniversalTime();
                                itemLCBase.AtualizadoBanco = false;
                                await DatabaseService.Database.SalvarListaCompra(itemLCBase);

                            }
                        }
                        if (ItemItemCompra.Identificador > 0)
                        {

                            await DatabaseService.Database.SalvarItemCompra(ItemItemCompra);
                        }
                        else
                            await DatabaseService.Database.ExcluirItemCompra(ItemItemCompra);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Item excluído com sucesso" } };

                    }



                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<ItemCompra>(MessageKeys.ManutencaoItemCompra, ItemItemCompra);
                    await PopAsync();



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
            List<Usuario> ListaUsuario = new List<Usuario>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {

                    ListaUsuario = await srv.ListarParticipantesViagem();
                    if (!ListaUsuario.Any())
                        ListaUsuario = await DatabaseService.Database.ListarParticipanteViagem();
                }
            }
            else
            {
                ListaUsuario = await DatabaseService.Database.ListarParticipanteViagem();
            }
            ListaAmigos = new ObservableCollection<Usuario>(ListaUsuario);
            OnPropertyChanged("ListaAmigos");
            int? IdentificadorRefeicaoSelecionada = ItemItemCompra.IdentificadorUsuario;
            ItemItemCompra.IdentificadorUsuario = null;
            await Task.Delay(100);
            ItemItemCompra.IdentificadorUsuario = IdentificadorRefeicaoSelecionada;
        }

        public async void CarregarListaCompra()
        {
            List<ListaCompra> ListaDados = new List<ListaCompra>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ListaDados = await srv.CarregarListaPedidos(new CriterioBusca() { IdentificadorParticipante = ItemUsuarioLogado.Codigo });
                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
                ListaDados = await DatabaseService.Database.ListarListaCompra(ItemUsuarioLogado.Codigo);
            ListaListaCompra = new ObservableCollection<ListaCompra>(ListaDados);
            OnPropertyChanged("ListaListaCompra");
            int? IdentificadorRefeicaoSelecionada = ItemItemCompra.IdentificadorListaCompra;
            ItemItemCompra.IdentificadorListaCompra = null;
            await Task.Delay(100);
            ItemItemCompra.IdentificadorListaCompra = IdentificadorRefeicaoSelecionada;
            await Task.Delay(100);
            _Loading = false;

        }


        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                bool Executado = false;
                ResultadoOperacao Resultado = new ResultadoOperacao();
                if (Conectado)
                {
                    try
                    {
                        Resultado = await SalvarItemCompraConectado();
                        Executado = true;
                    }
                    catch { Executado = false; }
                }
                if (!Executado)
                {
                    Resultado = await DatabaseService.SalvarItemCompra(ItemItemCompra, _IdentificadorListaCompraInicial);
                }
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
            finally
            {
                IsBusy = false;

                SalvarCommand.ChangeCanExecute();
            }
        }

        private async Task<ResultadoOperacao> SalvarItemCompraConectado()
        {
            ResultadoOperacao Resultado = null;
            using (ApiService srv = new ApiService())
            {
                Resultado = await srv.SalvarItemCompra(ItemItemCompra);
                if (Resultado.Sucesso)
                {
                    var itemCompra = await srv.CarregarItemCompra(Resultado.IdentificadorRegistro);
                    AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "IC", Resultado.IdentificadorRegistro.GetValueOrDefault(), !ItemItemCompra.Identificador.HasValue);
                    var itemBase = await DatabaseService.Database.RetornarItemCompra(Resultado.IdentificadorRegistro);
                    if (itemBase != null)
                    {
                        itemCompra.Id = itemBase.Id;
                    }
                    await DatabaseService.Database.SalvarItemCompra(itemCompra);
                    if (_IdentificadorListaCompraInicial.GetValueOrDefault(0) != ItemItemCompra.IdentificadorListaCompra.GetValueOrDefault())
                    {
                        if (ItemItemCompra.IdentificadorListaCompra.HasValue)
                        {
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", ItemItemCompra.IdentificadorListaCompra.GetValueOrDefault(), false);
                            var itemLC = await DatabaseService.Database.RetornarItemCompra(ItemItemCompra.IdentificadorListaCompra.GetValueOrDefault());
                            var itemLCServer = await srv.CarregarListaCompra(ItemItemCompra.IdentificadorListaCompra);
                            if (itemLC != null)
                                itemLCServer.Id = itemLC.Id;
                            await DatabaseService.Database.SalvarListaCompra(itemLCServer);
                        }
                        if (_IdentificadorListaCompraInicial.HasValue)
                        {
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", _IdentificadorListaCompraInicial.GetValueOrDefault(), false);
                            var itemLC = await DatabaseService.Database.RetornarItemCompra(_IdentificadorListaCompraInicial.GetValueOrDefault());
                            var itemLCServer = await srv.CarregarListaCompra(_IdentificadorListaCompraInicial);
                            if (itemLC != null)
                                itemLCServer.Id = itemLC.Id;
                            await DatabaseService.Database.SalvarListaCompra(itemLCServer);
                        }
                    }
                }
            }

            return Resultado;
        }
    }
}
