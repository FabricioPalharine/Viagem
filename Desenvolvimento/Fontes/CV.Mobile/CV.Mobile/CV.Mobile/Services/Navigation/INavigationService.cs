using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.ViewModels;
using CV.Mobile.ViewModels.Base;
using Xamarin.Forms;

namespace CV.Mobile.Services.Navigation
{
    public interface INavigationService
    {
        Task IniciarNavegacao<T>(object item=null) where T : BaseViewModel;

        Task TrocarPaginaShell(string Caminho, object item = null);   
        Task<Page> CreatePageAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;
    }
}
