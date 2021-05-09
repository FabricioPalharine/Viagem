using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Services.Dependency;
using CV.Mobile.Services.Dialog;
using CV.Mobile.Services.Navigation;
using CV.Mobile.Services.Settings;
using Xamarin.Forms;
using TinyIoC;
using CV.Mobile.Services.RequestProvider;
using CV.Mobile.Services.GoogleToken;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.GPS;
using CV.Mobile.Services.Sincronizacao;
using CV.Mobile.Services.Fotos;

namespace CV.Mobile.ViewModels.Base
{
    public static class ViewModelLocator
    {
        private static TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        public static bool UseMockService { get; set; }

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();

            // View models - by default, TinyIoC will register concrete classes as multi-instance.
            _container.Register<LoginViewModel>();
            _container.Register<AppShellViewModel>();
            _container.Register<HomeViewModel>();
            _container.Register<ConfiguracaoViewModel>();
            _container.Register<Amigos.AmigoAdicaoViewModel>();
            _container.Register<Amigos.AmigoListaViewModel>();
            _container.Register<Amigos.AmigoRequisicaoViewModel>();
            _container.Register<Amigos.AmigosViewModel>();
            _container.Register<Atracoes.AtracaoEdicaoViewModel>();
            _container.Register<Atracoes.AtracaoFiltroViewModel>();
            _container.Register<Atracoes.AtracaoListaViewModel>();
            _container.Register<Carros.CarroEdicaoViewModel>();
            _container.Register<Carros.CarroFiltroViewModel>();
            _container.Register<Carros.CarroListaViewModel>();
            _container.Register<Carros.CarroReabastecimentoViewModel>();
            _container.Register<Comentarios.ComentarioEdicaoViewModel>();
            _container.Register<Comentarios.ComentarioFiltroViewModel>();
            _container.Register<Comentarios.ComentarioListaViewModel>();
            _container.Register<Consultas.AcertoContaFiltroViewModel>();
            _container.Register<Consultas.AcertoContaViewModel>();
            _container.Register<Consultas.CalendarioRealizadoFiltroViewModel>();
            _container.Register<Consultas.CalendarioRealizadoViewModel>();
            _container.Register<Consultas.ExtratoDinheiroFiltroViewModel>();
            _container.Register<Consultas.ExtratoDinheiroViewModel>();
            _container.Register<Consultas.GaleriaFotoFiltroViewModel>();
            _container.Register<Consultas.GaleriaFotoViewModel>();
            _container.Register<Consultas.LocaisFiltroViewModel>();
            _container.Register<Consultas.LocaisViewModel>();
            _container.Register<Consultas.LocaisDetalheViewModel>();
            _container.Register<Consultas.ConsultaMapaFiltroViewModel>();
            _container.Register<Consultas.ConsultaMapaViewModel>();
            _container.Register<Consultas.RelatorioGastoFiltroViewModel>();
            _container.Register<Consultas.RelatorioGastoViewModel>();
            _container.Register<Consultas.TimelineFiltroViewModel>();
            _container.Register<Consultas.TimelineViewModel>();
            _container.Register<Deslocamentos.DeslocamentoEdicaoViewModel>();
            _container.Register<Deslocamentos.DeslocamentoFiltroViewModel>();
            _container.Register<Deslocamentos.DeslocamentoListaViewModel>();
            _container.Register<Gastos.GastoEdicaoViewModel>();
            _container.Register<Gastos.GastoFiltroViewModel>();
            _container.Register<Gastos.GastoListaViewModel>();
            _container.Register<Gastos.GastoSelecaoViewModel>();

            _container.Register<Hoteis.HotelEdicaoViewModel>();
            _container.Register<Hoteis.HotelFiltroViewModel>();
            _container.Register<Hoteis.HotelListaViewModel>();
            _container.Register<Lojas.LojaCompraViewModel>();
            _container.Register<Lojas.LojaEdicaoViewModel>();
            _container.Register<Lojas.LojaFiltroViewModel>();
            _container.Register<Lojas.LojaItemCompraViewModel>();
            _container.Register<Lojas.LojaListaViewModel>();
            _container.Register<Moedas.MoedaCotacaoEdicaoViewModel>();
            _container.Register<Moedas.MoedaCotacaoViewModel>();
            _container.Register<Moedas.MoedaEdicaoViewModel>();
            _container.Register<Moedas.MoedaFiltroViewModel>();
            _container.Register<Moedas.MoedaListaViewModel>();
            _container.Register<Moedas.MoedaCotacaoFiltroViewModel>();
            _container.Register<Mapas.MapaExibicaoViewModel>();
            _container.Register<Mapas.MapaSelecaoViewModel>();

            _container.Register<Refeicoes.RefeicaoEdicaoViewModel>();
            _container.Register<Refeicoes.RefeicaoFiltroViewModel>();
            _container.Register<Refeicoes.RefeicaoListaViewModel>();
            _container.Register<Viagens.ViagemEdicaoViewModel>();
            _container.Register<Viagens.ViagemFiltroViewModel>();
            _container.Register<Viagens.ViagemListaViewModel>();
            _container.Register<Consultas.ConsultaFotoDetalheViewModel>();
            _container.Register<SincronizacaoViewModel>();
            _container.Register<ComentarioFotoViewModel>();


            // Services - by default, TinyIoC will register interface registrations as singletons.
            _container.Register<INavigationService, NavigationService>();
            _container.Register<IDialogService, DialogService>();
            _container.Register<IRequestProvider, RequestProviderService>();
            _container.Register<IDependencyService, Services.Dependency.DependencyService>();
            _container.Register<ISettingsService, SettingsService>();
            _container.Register<IAccountSevice, AccountService>();
            _container.Register<IApiService, ApiService>();
            _container.Register<IDataService, DataService>();
            _container.Register<IDatabase, CVDatabase>();
            _container.Register<IGPSService, GPSService>();
            _container.Register<ISincronizacao, SincronizacaoService>();
            _container.Register<IFoto, FotoService>();

        }

        public static void UpdateDependencies(bool useMockServices)
        {
            // Change injected dependencies
            if (useMockServices)
            {
            

                UseMockService = true;
            }
            else
            {


                UseMockService = false;
            }
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>().AsSingleton();
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);
            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType==null)
            {
                viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName.Replace("Page","ViewModel"), viewAssemblyName);
                viewModelType = Type.GetType(viewModelName);
            }
            if (viewModelType == null)
            {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}
