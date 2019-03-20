using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using CV.Mobile.Helpers;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Xml;

namespace CV.Mobile.ViewModels
{
    public class VideoYouTubeViewModel : BaseNavigationViewModel
    {

        private VideoYouTube _ItemSelecionado;
        private string TokenProximaPagina { get; set; }
        YouTubeService youtube;
        public VideoYouTubeViewModel(Viagem pitemViagem, Usuario itemUsuario)
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

            youtube = new YouTubeService(service);
            ItemViagem = pitemViagem;
            ListaDados = new ObservableCollection<VideoYouTube>();
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarVideos();
                                                                   },
                                                                   () => true);

            AdicionarCommand = new Command(
                                                                   async () => await CarregarVideos(),
                                                                   () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  (obj) => { });

            DeleteCommand = new Command<VideoYouTube>(
                                                                   (obj) => VerificarExclusao(obj));

       
        
        }

        private async Task CarregarVideos()
        {
            await Task.Delay(1);
            ListaDados.Clear();
            var videoSearch = youtube.Search.List("snippet");
            videoSearch.ForMine = true;
            videoSearch.Type = "video";
            videoSearch.MaxResults = 10;
            if (!string.IsNullOrEmpty(TokenProximaPagina))
                videoSearch.PageToken = TokenProximaPagina;
            var result = await videoSearch.ExecuteAsync();
            TokenProximaPagina = result.NextPageToken;
            foreach(var item in result.Items)
            {
                VideoYouTube video = new VideoYouTube();
                video.VideoId = item.Id.VideoId;
                video.Titulo = item.Snippet.Title;
                video.VideoData = item.Snippet.PublishedAt.GetValueOrDefault(DateTime.Today);
                video.LinkThumbnail = item.Snippet.Thumbnails.Medium.Url;

                ListaDados.Add(video);
            }
        }

        private void VerificarExclusao(VideoYouTube obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja adicionar o vídeo {0}?", obj.Titulo),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;


                    using (ApiService srv = new ApiService())
                    {
                        UploadFoto itemUpload = new UploadFoto() { CodigoGoogle = obj.VideoId, DataArquivo = obj.VideoData, Thumbnail = obj.LinkThumbnail, Video = true, NomeArquivo = obj.Titulo };

                        var vidstream = youtube.Videos.List("snippet,Player,FileDetails");
                        vidstream.Id = obj.VideoId;
                        VideoListResponse ser = vidstream.Execute();
                        var vid = ser.Items.ToList();
                        if (vid.Count() > 0)
                        {

                            XmlReader xmlReader = XmlReader.Create(new StringReader(vid.FirstOrDefault().Player.EmbedHtml.Replace("allowfullscreen", null)));
                            xmlReader.MoveToContent();
                            itemUpload.LinkGoogle = xmlReader.GetAttribute("src");
                            itemUpload.ImageMime = vid.FirstOrDefault().FileDetails.FileType;

                            var resultadoAPI = await srv.SubirVideo(itemUpload);
                            if (resultadoAPI.Sucesso)
                            {
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "F", resultadoAPI.IdentificadorRegistro.GetValueOrDefault(), true);




                                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                                {
                                    Title = "Sucesso",
                                    Message = String.Join(Environment.NewLine, resultadoAPI.Mensagens.Select(d => d.Mensagem).ToArray()),
                                    Cancel = "OK"
                                });
                            }
                            else
                            {
                                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                                {
                                    Title = "Erro",
                                    Message = String.Join(Environment.NewLine, resultadoAPI.Mensagens.Select(d => d.Mensagem).ToArray()),
                                    Cancel = "OK"
                                });
                            }
                        }

                    }
                })
            });
        }

        public Viagem ItemViagem { get; set; }



        public ObservableCollection<VideoYouTube> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }



        public VideoYouTube ItemSelecionado
        {
            get
            {
                return _ItemSelecionado;
            }

            set
            {
                _ItemSelecionado = null;
                OnPropertyChanged();
            }
        }

       

    }
}
