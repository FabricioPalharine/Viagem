using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CV.Mobile.Models;
using Xamarin.Forms;
using Xamarin.Auth;
using CV.Mobile.Interfaces;
using Microsoft.Practices.ServiceLocation;
using CV.Mobile.Views;
using FormsToolkit;
using CV.Mobile.Services;

namespace CV.Mobile.ViewModels
{
    public class MenuInicialViewModel:BaseNavigationViewModel
    {

        public MenuInicialViewModel()
        {
            this.Title = "Curtindo uma Viagem";
            DeslogarCommand =  new Command(
                                    async () => await DeslogarAplicacao(),
                                    () => true);
            EntrarViagemCommand = new Command(
                                    async () => await EntraViagem(),
                                    () => true);
            CriarViagemCommand = new Command(
                                    async () => await CriarViagem(),
                                    () => true);
            AbrirAmigosCommand = new Command(
                                    async () => await AbrirViewAmigo(),
                                    () => true);
        }

        private async Task DeslogarAplicacao()
        {
            IValidaAutenticacao _validador = ServiceLocator.Current.GetInstance<IValidaAutenticacao>();
            var autenticacao = await _validador.RetornarAutenticacaoAplicacao();
            await AccountStore.Create().DeleteAsync(autenticacao, Constants.AppName);
            LoadingViewModel vm = new LoadingViewModel();
            App.Current.MainPage = new LoadingPage() { BindingContext = vm };
        }

        private async Task EntraViagem()
        {
            if (await VerificarOnline())
            {
                var Pagina = new SelecionarViagemPage() { BindingContext = new SelecionarViagemViewModel() };
                await PushAsync(Pagina);
            }
        }

        private async Task CriarViagem()
        {
            if (await VerificarOnline())
            {
                var ItemUsuario = ((MasterDetailViewModel)Application.Current?.MainPage.BindingContext).ItemUsuario;

                Viagem _ViagemSelecionada = new Viagem() { IdentificadorUsuario = ItemUsuario.Codigo, PublicaGasto = false, DataInicio = DateTime.Today, DataFim = DateTime.Today, QuantidadeParticipantes = 1, UnidadeMetrica = true, Participantes = new MvvmHelpers.ObservableRangeCollection<ParticipanteViagem>(), UsuariosGastos = new MvvmHelpers.ObservableRangeCollection<UsuarioGasto>(), Moeda = 790, Aberto = true };
                _ViagemSelecionada.Participantes.Add(new ParticipanteViagem() { IdentificadorUsuario = ItemUsuario.Codigo, NomeUsuario = ItemUsuario.Nome, ItemUsuario = new Usuario() { Identificador = ItemUsuario.Codigo, Nome = ItemUsuario.Nome }, PermiteExcluir = false });

                var Pagina = new EditarViagemPage() { BindingContext = new EditarViagemViewModel(_ViagemSelecionada), Title = "Nova Viagem" };
                await PushAsync(Pagina);
            }
        }

        private async Task AbrirViewAmigo()
        {
            if (await VerificarOnline())
            {
                var Pagina = new ListagemAmigoPage() { BindingContext = new ListagemAmigoViewModel() };
                await PushAsync(Pagina);
            }
        }

        public Command SelecionarViagemCommand { get; set; }
        public Command AbrirAmigosCommand { get; set; }
        public Command CriarViagemCommand { get; set; }
        public Command EntrarViagemCommand { get; set; }
        public Command AbrirRankingCommand { get; set; }
        public Command AbrirConfiguracaoCommand { get; set; }
        public Command DeslogarCommand { get; set; }

        public bool ViagemSelecionada
        {
            get
            {
                return _ViagemSelecionada;
            }

            set
            {
                SetProperty(ref _ViagemSelecionada, value);
            }
        }

        public Viagem ItemViagem
        {
            get
            {
                return _ItemViagem;
            }

            set
            {
                SetProperty(ref _ItemViagem, value);
                ViagemSelecionada = value != null;
                if (ViagemSelecionada !=EntrarViagemCommand.CanExecute(null))
                    EntrarViagemCommand.ChangeCanExecute();
                 

            }
        }

        private bool _ViagemSelecionada;

        private Viagem _ItemViagem;

        private void ExibirAlertaOffLine()
        {
            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
            {
                Title = "Conexão Off-Line",
                Message = "Essa funcionalidade necessita que a conexão esteja On-Line",
                Cancel = "OK"
            });
        }

        private async Task<bool> VerificarOnline()
        {
            bool Online = false;
            using (ApiService srv = new ApiService())
            {
                Online = await srv.VerificarOnLine();
            }
            if (!Online)
                ExibirAlertaOffLine();
            return Online;
        }
    }
}
