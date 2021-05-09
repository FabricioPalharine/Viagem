using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class HomeViewModel: BaseViewModel
    {
        public HomeViewModel()
        {

        }

        public ICommand ConfiguracoesCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ConfiguracaoPage", null);

        });

        public ICommand AmigosCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("AmigosPage", null);
        });


        public ICommand CriarViagemCommand => new Command(async () =>
        {
            string Caminho = "/ViagemCriacaoPage";
            await NavigationService.TrocarPaginaShell(Caminho, null);


        });

        public ICommand SelecionarViagemCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ViagemListaPage", null);

        });
    }
}
