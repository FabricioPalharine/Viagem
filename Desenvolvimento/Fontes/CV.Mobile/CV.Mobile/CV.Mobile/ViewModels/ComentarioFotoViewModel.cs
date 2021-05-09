using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Dependency;
using CV.Mobile.Services.Fotos;
using CV.Mobile.Services.GoogleToken;
using CV.Mobile.Services.PlatformSpecifcs;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using MediaManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ComentarioFotoViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IFoto _foto;
        private readonly IDatabase _database;
        private readonly IAccountSevice _accountSevice;
        private readonly ISettingsService _settingsService;
        private readonly IFileHelper _fileHelper;
        private ValidatableObject<string> _texto = new ValidatableObject<string>();
        private UploadFoto imagemUpload = new UploadFoto();
        private ImageSource _imageSource = null;
        private bool _video = false;
        private object _videoSource = null;
         
        public ComentarioFotoViewModel(IApiService apiService, IFoto foto, IDatabase database, IAccountSevice accountSevice, ISettingsService settingsService, IDependencyService dependency)
        {
            _apiService = apiService;
            _foto = foto;
            _database = database;
            _accountSevice = accountSevice;
            _settingsService = settingsService;
            _fileHelper = dependency.Get<IFileHelper>();
        }

        public ValidatableObject<string> Texto
        {
            get { return _texto; }
            set { SetProperty(ref _texto, value); }
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        public bool Video
        {
            get { return _video; }
            set { SetProperty(ref _video, value); }
        }

        public object VideoSource
        {
            get { return _videoSource; }
            set { SetProperty(ref _videoSource, value); }
        }  

        public async override Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is UploadFoto item)
            {
                imagemUpload = item;
                Video = item.Video;
                if (!item.Video)
                {
                    var stream = await item.file.OpenReadAsync();
                    ImageSource = ImageSource.FromStream(() => { return stream; });
                }
                else
                {

                    var video = await CrossMediaManager.Current.Extractor.CreateMediaItem(_fileHelper.RetornarFile(imagemUpload.file.FullPath));
                    VideoSource = video;

                }

            }            
        }

        public ICommand CancelarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));

        public ICommand SalvarCommand => new Command(async () =>
        {
            string configuracao = Video ? _settingsService.ModoVideo : _settingsService.ModoImagem;
            imagemUpload.Comentario = Texto.Value;
            if (Funcoes.AcessoInternet && (configuracao == "1" || (configuracao == "2" && Funcoes.AcessoRede)))
            {
                UploadFoto();
            }
            else
            {
                await _database.SalvarUploadFoto(imagemUpload);
            }
            await NavigationService.TrocarPaginaShell("..");

        });

        private async void UploadFoto()
        {
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            await _accountSevice.AtualizarToken();

            var stream = await imagemUpload.file.OpenReadAsync();
            using (var newStream = new MemoryStream())
            {
                await stream.CopyToAsync(newStream);
                byte[] DadosFoto = newStream.ToArray();
               
                    await _foto.SubirFoto(usuario.Properties["access_token"], GlobalSetting.Instance.ViagemSelecionado.CodigoAlbum, DadosFoto, imagemUpload);
                    await _apiService.SubirImagem(imagemUpload);
                
            }
        }

        public ICommand PageDisappearingCommand => new Command(async () => await CrossMediaManager.Current.Stop());

    }
}
