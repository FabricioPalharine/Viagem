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
                    itemUpload.CaminhoLocal = retorno.Path;
                    itemUpload.ImageMime = "image/jpeg";
                    var Source = retorno.GetStream();
                    var exif = ExifReader.ReadJpeg(Source);

                    Source.Seek(0, SeekOrigin.Begin);
                    itemUpload.DataArquivo = DateTime.Now;
                    if (exif != null)
                    {
                        itemUpload.Latitude = (exif.DecimalLatitude());
                        itemUpload.Longitude = (exif.DecimalLongitude());
                    }
                    var itemComentario = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Comentários", "", "Ok", "Cancelar");
                    if (itemComentario.Ok)
                        itemUpload.Comentario = itemComentario.Value;
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
                        await srv.SubirImagem(itemUpload);
                    }
                }
            }
        }
        #endregion
    }
}
