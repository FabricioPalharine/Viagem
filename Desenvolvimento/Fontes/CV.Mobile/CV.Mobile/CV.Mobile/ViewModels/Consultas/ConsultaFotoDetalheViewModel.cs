using CV.Mobile.Models;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Consultas
{
    public class ConsultaFotoDetalheViewModel: BaseViewModel
    {
        private bool _video = false;
        private string _url = string.Empty;
        private string _comentario = null;
        private object _videoSource = null;

        public async override Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is Foto item)
            {
                Video = item.Video;
                URL = item.LinkFoto;
                Comentario = item.Comentario;
                if  (Video)
                {
                    var video = await CrossMediaManager.Current.Extractor.CreateMediaItem(item.LinkFoto);

                    VideoSource = video;
                }
            }
            else
                await NavigationService.TrocarPaginaShell("..");
        }
        public object VideoSource
        {
            get { return _videoSource; }
            set { SetProperty(ref _videoSource, value); }
        }

        public bool Video
        {
            get { return _video; }
            set { SetProperty(ref _video, value); }
        }

        public string URL
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public string Comentario
        {
            get { return _comentario; }
            set { SetProperty(ref _comentario, value); }
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand PageDisappearingCommand => new Command(async () => await CrossMediaManager.Current.Stop());

    }
}
