using CV.Mobile.Helper;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Dependency;
using CV.Mobile.Services.Fotos;
using CV.Mobile.Services.GoogleToken;
using CV.Mobile.Services.PlatformSpecifcs;
using CV.Mobile.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Services.Sincronizacao
{
    public class SincronizacaoService : ISincronizacao
    {
        private readonly IApiService _apiService;
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IFoto _foto;
        private readonly IFileHelper _fileHelper;
        private readonly IAccountSevice _account;

        private bool sincronizando = false;

        public SincronizacaoService(IApiService apiService, IDatabase database, IDataService dataService, ISettingsService settingsService, IFoto foto,
             IDependencyService dependencyService, IAccountSevice account)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            _foto = foto;
            _fileHelper = dependencyService.Get<IFileHelper>();
            _account = account;
            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
            {
                VerificarSincronizar();
                return true;
            });
        }

        private async void VerificarSincronizar()
        {
            if (Funcoes.AcessoInternet && !sincronizando)
                await SincronizarInterno(false);
        }

        public async Task<bool> Sincronizar(bool Comandada)
        {
            return await SincronizarInterno(Comandada);
        }

        private async Task<bool> SincronizarInterno(bool Comandada)
        {
            bool Sincronizado = true;
            sincronizando = true;
            if (Comandada ||  _settingsService.ModoSincronizacao == "1" || (_settingsService.ModoSincronizacao == "2" && Funcoes.AcessoRede))
            {
                var item = await _dataService.CarregarDadosEnvioSincronizar();
                var resultadoSincronizacao = await _apiService.SincronizarDados(item);  
                if (resultadoSincronizacao != null)
                    await _dataService.AjustarDePara(item, resultadoSincronizacao);
            }
            if (Comandada || _settingsService.ModoImagem == "1" || (_settingsService.ModoImagem == "2" && Funcoes.AcessoRede))
            {
                await SincronizarFotosVideos(false);
            }
            if (Comandada || _settingsService.ModoVideo == "1" || (_settingsService.ModoVideo == "2" && Funcoes.AcessoRede))
            {
                await SincronizarFotosVideos(true);
            }
            sincronizando = false;
            return Sincronizado;
        }

        private async Task SincronizarFotosVideos(bool Video)
        {
            var listaFotos = await _database.ListarUploadFoto_Video(Video);
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            await _account.AtualizarToken();
            foreach (var itemFoto in listaFotos)
            {
                byte[] DadosFoto = _fileHelper.CarregarDadosFile(itemFoto.CaminhoLocal);
                await _foto.SubirFoto(usuario.Properties["access_token"], GlobalSetting.Instance.ViagemSelecionado.CodigoAlbum, DadosFoto, itemFoto);
                await _apiService.SubirImagem(itemFoto);
                await _database.ExcluirUploadFoto(itemFoto);
            }
        }
    }
}
