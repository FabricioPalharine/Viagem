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
        }

        private async Task DeslogarAplicacao()
        {
            IValidaAutenticacao _validador = ServiceLocator.Current.GetInstance<IValidaAutenticacao>();
            var autenticacao = await _validador.RetornarAutenticacaoAplicacao();
            await AccountStore.Create().DeleteAsync(autenticacao, Constants.AppName);
            LoadingViewModel vm = new LoadingViewModel();
            App.Current.MainPage = new LoadingPage() { BindingContext = vm };
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
    }
}
