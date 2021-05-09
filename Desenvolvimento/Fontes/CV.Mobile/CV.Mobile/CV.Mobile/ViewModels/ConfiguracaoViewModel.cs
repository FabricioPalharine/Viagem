using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ConfiguracaoViewModel : BaseViewModel
    {
        private ValidatableObject<ItemLista> _manterOnline = new ValidatableObject<ItemLista>();
        private ValidatableObject<ItemLista> _sincronizarDados = new ValidatableObject<ItemLista>();
        private ValidatableObject<ItemLista> _enviarImagens = new ValidatableObject<ItemLista>();
        private ValidatableObject<ItemLista> _enviarVideos = new ValidatableObject<ItemLista>();
        private ObservableCollection<ItemLista> _listaTipoOnline = new ObservableCollection<ItemLista>();
        private ObservableCollection<ItemLista> _tiposAtualizacao = new ObservableCollection<ItemLista>();
        private readonly ISettingsService _settingsService;
        public ConfiguracaoViewModel(ISettingsService settingsService)
        {
            TiposAtualizacao.Add(new ItemLista() { Codigo = "1", Descricao = "Rede Móvel" });
            TiposAtualizacao.Add(new ItemLista() { Codigo = "2", Descricao = "Rede WiFi" });
            TiposAtualizacao.Add(new ItemLista() { Codigo = "3", Descricao = "Comando Manual" });

            ListaTipoOnline.Add(new ItemLista() { Codigo = "1", Descricao = "Rede Móvel" });
            ListaTipoOnline.Add(new ItemLista() { Codigo = "2", Descricao = "Rede WiFi" });
            ListaTipoOnline.Add(new ItemLista() { Codigo = "3", Descricao = "Nunca" });
            _settingsService = settingsService;

        }

        public override Task InitializeAsync(object navigationData)
        {
            ManterOnline.Value = ListaTipoOnline.Where(d => d.Codigo == _settingsService.AcompanhamentoOnline).FirstOrDefault();
            SincronizarDados.Value = TiposAtualizacao.Where(d => d.Codigo == _settingsService.ModoSincronizacao).FirstOrDefault();
            EnviarImagens.Value = TiposAtualizacao.Where(d => d.Codigo == _settingsService.ModoImagem).FirstOrDefault();
            EnviarVideos.Value = TiposAtualizacao.Where(d => d.Codigo == _settingsService.ModoVideo).FirstOrDefault();

            return base.InitializeAsync(navigationData);
        }

        public ValidatableObject<ItemLista> ManterOnline
        {
            get { return _manterOnline; }
            set { SetProperty(ref _manterOnline, value); }
        }
        public ValidatableObject<ItemLista> SincronizarDados
        {
            get { return _sincronizarDados; }
            set { SetProperty(ref _sincronizarDados, value); }
        }

        public ValidatableObject<ItemLista> EnviarImagens
        {
            get { return _enviarImagens; }
            set { SetProperty(ref _enviarImagens, value); }
        }

        public ValidatableObject<ItemLista> EnviarVideos
        {
            get { return _enviarVideos; }
            set { SetProperty(ref _enviarVideos, value); }
        }

        public ObservableCollection<ItemLista> ListaTipoOnline
        {
            get { return _listaTipoOnline; }
            set { SetProperty(ref _listaTipoOnline, value); }
        }

        public ObservableCollection<ItemLista> TiposAtualizacao
        {
            get { return _tiposAtualizacao; }
            set { SetProperty(ref _tiposAtualizacao, value); }
        }

        public ICommand ValidarManterOnlineCommand => new Command(() =>
        {
        });
        public ICommand ValidarSincronizarDadosCommand => new Command(() =>
        {
        });
        public ICommand ValidarEnviarImagensCommand => new Command(() =>
        {
        });
        public ICommand ValidarEnviarVideosCommand => new Command(() =>
        {
        });

        public ICommand SalvarCommand => new Command(async () =>
         {
             await Salvar();
         });

        private async Task Salvar()
        {
            _settingsService.ModoImagem = EnviarImagens.Value.Codigo;
            _settingsService.ModoVideo = EnviarVideos.Value.Codigo;
            _settingsService.ModoSincronizacao = SincronizarDados.Value.Codigo;
            _settingsService.AcompanhamentoOnline = ManterOnline.Value.Codigo;
            await DialogService.ShowAlertAsync(AppResource.ConfiguracaoSalvaSucesso, AppResource.AppName, AppResource.Ok);
            await NavigationService.TrocarPaginaShell(".."); 
            
        }
    }
}
