using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Services.Settings;
using CV.Mobile.ViewModels;
using CV.Mobile.ViewModels.Base;
using CV.Mobile.Views;
using Xamarin.Forms;

namespace CV.Mobile.Services.Navigation
{
    public class NavigationService : INavigationService
    {

        public async Task IniciarNavegacao<T>(object item = null) where T : BaseViewModel
        {
            Page page = CreatePage(typeof(T), item);
            (App.Current as App).MainPage = page;
            await (page.BindingContext as BaseViewModel).InitializeAsync(item);


        }

        public async Task TrocarPaginaShell(string Caminho, object item = null)
        {
            
                await Shell.Current.GoToAsync(Caminho);
                //await Task.Delay(500);
                if (Caminho != "..")
                    await (Shell.Current.CurrentPage.BindingContext as BaseViewModel).InitializeAsync(item);
           
        }


        public async Task<Page> CreatePageAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            Page page = CreatePage(typeof(TViewModel), parameter);
            await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
            return page;
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            if (viewType == null)
            {
                var posicao = viewName.LastIndexOf(".");
                var nome = viewName.Substring(posicao).Replace("View", "Page");
                viewName = string.Concat(viewName.Substring(0, posicao), nome);
                viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
                viewType = Type.GetType(viewAssemblyName);

            }
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }


    }
}
