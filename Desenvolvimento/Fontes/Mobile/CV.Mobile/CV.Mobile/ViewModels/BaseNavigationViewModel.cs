using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Helpers;
using ExifLib;
using MvvmHelpers;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using FormsToolkit;
using System.Globalization;
using System.Collections;
using Xamarin.Auth;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using System.Xml;

namespace CV.Mobile.ViewModels
{
    public abstract class BaseNavigationViewModel : BaseViewModel, INavigation
    {

        //  INavigation _Navigation =>  Application.Current?.MainPage.Navigation;
        INavigation _Navigation
        {
            get
            {
                if (Application.Current?.MainPage is MasterDetailPage)
                    return ((MasterDetailPage)Application.Current?.MainPage).Detail?.Navigation;
                else
                    return Application.Current?.MainPage.Navigation;
            }
        }

        public async Task AtualizarViagem(int? Identificador)
        {
            using (ApiService srv = new ApiService())
            {
                Viagem itemViagem = await srv.CarregarViagem(Identificador);
                var DadosViagem = await srv.SelecionarViagem(itemViagem.Identificador);
                itemViagem.VejoGastos = DadosViagem.VerCustos;
                itemViagem.Edicao = DadosViagem.PermiteEdicao;
                itemViagem.Aberto = DadosViagem.Aberto;
                if (Application.Current?.MainPage is MasterDetailPage)
                {
                    ((MasterDetailViewModel)Application.Current?.MainPage.BindingContext).ItemViagem = itemViagem;
                }
                foreach (var Pagina in NavigationStack)
                {
                    if (Pagina.BindingContext is MenuInicialViewModel)
                    {
                        ((MenuInicialViewModel)Pagina.BindingContext).ViagemSelecionada = true;
                        ((MenuInicialViewModel)Pagina.BindingContext).ItemViagem = itemViagem;
                    }
                }
            }
        }

        public async Task<Position> RetornarPosicao()
        {
            if (Application.Current?.MainPage is MasterDetailPage)
            {
                return await ((MasterDetailViewModel)Application.Current?.MainPage.BindingContext).RetornarPosicaoGPS();
            }
            else
                return null;
        }

        public UsuarioLogado ItemUsuarioLogado
        {
            get
            {
                if (Application.Current?.MainPage is MasterDetailPage)
                    return (((MasterDetailPage)Application.Current?.MainPage).BindingContext as MasterDetailViewModel).ItemUsuario;
                else
                    return null;
            }
        }

        public Viagem ItemViagemSelecionada
        {
            get
            {
                if (Application.Current?.MainPage is MasterDetailPage)
                    return (((MasterDetailPage)Application.Current?.MainPage).BindingContext as MasterDetailViewModel).ItemViagem;
                else
                    return null;
            }
        }

        #region INavigation implementation

        public void RemovePage(Page page)
        {
            _Navigation?.RemovePage(page);
        }

        public void InsertPageBefore(Page page, Page before)
        {
            _Navigation?.InsertPageBefore(page, before);
        }

        public async Task PushAsync(Page page)
        {
            var task = _Navigation?.PushAsync(page);
            if (task != null)
                await task;
        }

        public async Task<Page> PopAsync()
        {
            var task = _Navigation?.PopAsync();
            return task != null ? await task : await Task.FromResult(null as Page);
        }

        public async Task PopToRootAsync()
        {
            var task = _Navigation?.PopToRootAsync();
            if (task != null)
                await task;
        }

        public async Task PushModalAsync(Page page)
        {
            var task = _Navigation?.PushModalAsync(page);
            if (task != null)
                await task;
        }

        public async Task<Page> PopModalAsync()
        {
            var task = _Navigation?.PopModalAsync();
            return task != null ? await task : await Task.FromResult(null as Page);
        }

        public async Task PushAsync(Page page, bool animated)
        {
            var task = _Navigation?.PushAsync(page, animated);
            if (task != null)
                await task;
        }

        public async Task<Page> PopAsync(bool animated)
        {
            var task = _Navigation?.PopAsync(animated);
            return task != null ? await task : await Task.FromResult(null as Page);
        }

        public async Task PopToRootAsync(bool animated)
        {
            var task = _Navigation?.PopToRootAsync(animated);
            if (task != null)
                await task;
        }

        public async Task PushModalAsync(Page page, bool animated)
        {
            var task = _Navigation?.PushModalAsync(page, animated);
            if (task != null)
                await task;
        }

        public async Task<Page> PopModalAsync(bool animated)
        {
            var task = _Navigation?.PopModalAsync(animated);
            return task != null ? await task : await Task.FromResult(null as Page);
        }

        public IReadOnlyList<Page> NavigationStack => _Navigation?.NavigationStack;

        public IReadOnlyList<Page> ModalStack => _Navigation?.ModalStack;

        #endregion

        #region Media

        protected async Task CarregarAcaoFoto(UploadFoto itemUpload)
        {
            await CrossMedia.Current.Initialize();
            List<string> Acoes = new List<string>(new string[]
            {
                        "Selecionar Foto",
                        "Selecionar Video"
            });
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                Acoes.Add("Tirar Foto");
            }
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakeVideoSupported)
            {
                Acoes.Add("Gravar Vídeo");
            }
            var action = await Application.Current.MainPage.DisplayActionSheet(string.Empty, "Cancelar", null,
              Acoes.ToArray()
             );
            if (action == "Selecionar Foto")
            {
                var retorno = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { CompressionQuality = 92 });
                if (retorno != null)
                {
                    await GravarFoto(itemUpload, retorno,false);
                }
            }
            else if (action == "Tirar Foto")
            {
                var retorno = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { CompressionQuality = 92, AllowCropping = true, DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear, SaveToAlbum=true });
                if (retorno != null)
                {
                    await GravarFoto(itemUpload, retorno,true);
                }
            }
            else if (action == "Selecionar Video")
            {
                var retorno = await CrossMedia.Current.PickVideoAsync();
                if (retorno != null)
                {
                    await GravarVideo(itemUpload, retorno,false);
                }
            }
            else if (action == "Gravar Vídeo")
            {
                var retorno = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions() {  AllowCropping = true, DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear, SaveToAlbum = true });
                if (retorno != null)
                {
                    await GravarVideo(itemUpload, retorno,true);
                }
            }
        }

        private async Task GravarFoto(UploadFoto itemUpload, Plugin.Media.Abstractions.MediaFile retorno,bool Novo)
        {
            itemUpload.CaminhoLocal = retorno.Path;
            itemUpload.ImageMime = "image/jpeg";
            var Source = retorno.GetStream();
            var exif = ExifReader.ReadJpeg(Source);
            
            Source.Seek(0, SeekOrigin.Begin);
            itemUpload.DataArquivo = DateTime.Now;
            if (exif != null && (exif.DecimalLatitude() != 0 || exif.DecimalLongitude() != 0))
            {
                
                itemUpload.Latitude = (exif.DecimalLatitude());
                itemUpload.Longitude = (exif.DecimalLongitude());
            }
            else if (Novo)
            {
                var posicao = await RetornarPosicao();
                if (posicao != null)
                {
                    itemUpload.Latitude = posicao.Latitude;
                    itemUpload.Longitude = posicao.Longitude;
                }

            }
            if (exif != null)
            {
                string Texto = exif.DateTime;
                DateTime dataConverte;
                if (!string.IsNullOrWhiteSpace(Texto) && DateTime.TryParseExact(Texto, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dataConverte))
                    itemUpload.DataArquivo = dataConverte;
            }
            var VM = new TextPopupViewModel() { Title = "Comentário", Texto = "Adicionar Comentário Foto" };
            var Pagina = new Views.TextoPopupPage() { BindingContext = VM };
            await PushModalAsync(Pagina, true);
            MessagingService.Current.Subscribe(MessageKeys.MensagemModalConfirmada, async (service) =>
            {
                MessagingService.Current.Unsubscribe(MessageKeys.MensagemModalConfirmada);

                if (VM.Confirmado)
                    itemUpload.Comentario = VM.Valor;
                byte[] DadosFoto = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    Source.CopyTo(ms);
                    DadosFoto = ms.ToArray();
                }
                using (ApiService srv = new ApiService())
                {
                    var ItemUsuario = await srv.CarregarUsuario(ItemUsuarioLogado.Codigo);
                    if (ItemUsuario.DataToken.GetValueOrDefault().ToUniversalTime().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now.ToUniversalTime())
                    {
                        using (AccountsService srvAccount = new AccountsService())
                        {
                            await srvAccount.AtualizarTokenUsuario(ItemUsuario);
                        }
                    }
                    using (PhotosService srvFoto = new PhotosService(ItemUsuario.Token))
                    {
                         await srvFoto.SubirFoto(ItemViagemSelecionada.CodigoAlbum, DadosFoto, itemUpload);
                    }
                    var resultadoAPI = await srv.SubirImagem(itemUpload);
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = "Foto Gravada com Sucesso",
                        Cancel = "OK"
                    });
                }

            });
            
        }

        private async Task GravarVideo(UploadFoto itemUpload, Plugin.Media.Abstractions.MediaFile retorno, bool Novo)
        {
            if (Novo)
            {
                var posicao = await RetornarPosicao();
                if (posicao != null)
                {
                    itemUpload.Latitude = posicao.Latitude;
                    itemUpload.Longitude = posicao.Longitude;
                }
                
            }
            itemUpload.CaminhoLocal = retorno.Path;
            itemUpload.ImageMime = "video/avi";

            itemUpload.DataArquivo = DateTime.Now;
            var Source = retorno.GetStream();
            var VM = new TextPopupViewModel() { Title = "Comentário", Texto = "Adicionar Comentário Foto" };
            var Pagina = new Views.TextoPopupPage() { BindingContext = VM };
            await PushModalAsync(Pagina, true);
            MessagingService.Current.Subscribe(MessageKeys.MensagemModalConfirmada, async (service) =>
            {
                MessagingService.Current.Unsubscribe(MessageKeys.MensagemModalConfirmada);

                if (VM.Confirmado)
                    itemUpload.Comentario = VM.Valor;
               
                using (ApiService srv = new ApiService())
                {
                    var ItemUsuario = await srv.CarregarUsuario(ItemUsuarioLogado.Codigo);
                    if (ItemUsuario.DataToken.GetValueOrDefault().ToUniversalTime().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now.ToUniversalTime())
                    {
                        using (AccountsService srvAccount = new AccountsService())
                        {
                            await srvAccount.AtualizarTokenUsuario(ItemUsuario);
                        }
                    }
                    GravarVideoYouTube(itemUpload, Source, ItemUsuario);
                    //using (PhotosService srvFoto = new PhotosService(ItemUsuario.Token))
                    //{
                    //    await srvFoto.SubirFoto(ItemViagemSelecionada.CodigoAlbum, DadosFoto, itemUpload);
                    //}
                   
                }

            });
        }


        public async void GravarVideoYouTube(UploadFoto itemUpload, Stream baseStream, Usuario itemUsuario)
        {
            TokenResponse tr = new TokenResponse() { AccessToken = itemUsuario.Token, ExpiresInSeconds = itemUsuario.Lifetime, Issued = DateTime.Now, RefreshToken = itemUsuario.RefreshToken };

            var service = new BaseClientService.Initializer();
            service.ApiKey = Constants.ClientAPI;
            var secrets = new ClientSecrets() { ClientId = "id", ClientSecret = "secret" };
            var initializer = new AuthorizationCodeFlow.Initializer("https://authorization_code.com", "https://token.com")
            {
                ClientSecrets = secrets,

            };
            UserCredential uc = new UserCredential(new AuthorizationCodeFlow(initializer), "103654916712612822987", tr);
            service.HttpClientInitializer = uc;

            var youtube = new YouTubeService(service);
            

            Video video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = itemUpload.CaminhoLocal;
            video.Snippet.Description = itemUpload.Comentario;
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted";
  
            var videosInsertRequest = youtube.Videos.Insert(video, "snippet,status", baseStream, "video/*");
            //videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
           // videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;
            var resultado = await videosInsertRequest.UploadAsync();
            if (resultado.Status == Google.Apis.Upload.UploadStatus.Completed)
            {
                var retorno = videosInsertRequest.ResponseBody;

                itemUpload.CodigoGoogle = retorno.Id;
                itemUpload.Thumbnail = retorno.Snippet.Thumbnails.Medium.Url;
                var vidstream = youtube.Videos.List("snippet,Player,FileDetails");
                vidstream.Id = retorno.Id;
                VideoListResponse ser = vidstream.Execute();
                List<Video> vid = ser.Items.ToList();
                XmlReader xmlReader = XmlReader.Create(new StringReader(vid.FirstOrDefault().Player.EmbedHtml.Replace("allowfullscreen",null)));
                xmlReader.MoveToContent();
                itemUpload.LinkGoogle = xmlReader.GetAttribute("src");
                itemUpload.ImageMime = vid.FirstOrDefault().FileDetails.FileType;
                using (ApiService srv = new ApiService())
                {
                    var resultadoAPI = await srv.SubirVideo(itemUpload);
                }
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Sucesso",
                    Message = "Vídeo Gravado com Sucesso",
                    Cancel = "OK"
                });
            }

        }
        #endregion
    }
}
