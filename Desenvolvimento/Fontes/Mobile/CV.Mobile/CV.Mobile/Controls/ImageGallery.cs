using CV.Mobile.Models;
using FFImageLoading.Forms;
using MvvmHelpers;
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class ImageGalleryControl:ContentView
    {
        public static readonly BindableProperty FotosProperty = BindableProperty.Create
           (nameof(Fotos), typeof(ObservableRangeCollection<Foto>), typeof(CalendarioControl), new ObservableRangeCollection<Foto>(), BindingMode.OneWay,
           propertyChanged: (bindable, oldvalue, newvalue) =>
           {
               var page = bindable as ImageGalleryControl;
               var lista = newvalue as ObservableRangeCollection<Foto>;

               //if (lista != null)
               //    lista.CollectionChanged += page.Lista_CollectionChanged;
               page.RenderizarComponente();


           });

        public ImageGalleryControl()
        {
            this.Init();
        }

        public ObservableRangeCollection<Foto> Fotos
        {
            get { return (ObservableRangeCollection<Foto>)this.GetValue(FotosProperty); }
            set { this.SetValue(FotosProperty, value); }

        }

        StackLayout stack = new StackLayout();
        CarouselView cv = new CarouselView();
        ScrollView sv = new ScrollView();
        StackLayout stackImages = new StackLayout();
        List<CachedImage> listaImages = new List<CachedImage>();
        Label lbl = new Label();
        private async void Init()
        {
            await System.Threading.Tasks.Task.Delay(500);
            cv.SetBinding(CarouselView.ItemsSourceProperty, "Fotos");
            cv.BindingContext = this;
            cv.HorizontalOptions = LayoutOptions.FillAndExpand;
            cv.HeightRequest = 400;
            cv.PositionSelected += Cv_PositionSelected;
            DataTemplate dt = new DataTemplate(() =>
            {
                CachedImage img = new CachedImage();
                img.SetBinding(CachedImage.SourceProperty, "LinkControle");
                TapGestureRecognizer objTapGestureRecognizer = new TapGestureRecognizer();
                objTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Identificador");
                objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped; ;
                img.GestureRecognizers.Add(objTapGestureRecognizer);
                return img;
            });
            cv.ItemTemplate = dt;
            stackImages.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(cv);
            stack.Children.Add(lbl);
            sv.Orientation = ScrollOrientation.Horizontal;
            sv.HeightRequest = 100;
            sv.Content = stackImages;
            stack.Children.Add(sv);

            this.Content = stack;
            RenderizarComponente();
        }

        private void RenderizarComponente()
        {
            stackImages.Children.Clear();
            listaImages.Clear();
            if (Fotos != null)
            {
                foreach (Foto itemFoto in Fotos)
                {
                    CachedImage img = new CachedImage();
                    img.SetBinding(CachedImage.SourceProperty, "LinkThumbnail");
                    img.WidthRequest = 100;
                    img.HeightRequest = 100;
                    img.DownsampleToViewSize = true;
                    img.BindingContext = itemFoto;
                    TapGestureRecognizer objTapGestureRecognizer = new TapGestureRecognizer();
                    objTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Identificador");
                    objTapGestureRecognizer.BindingContext = itemFoto;
                    objTapGestureRecognizer.Tapped += SelecionarFotoControle;
                    img.GestureRecognizers.Add(objTapGestureRecognizer);
                    listaImages.Add(img);
                    stackImages.Children.Add(img);

                }
            }
        }

        private void SelecionarFotoControle(object sender, EventArgs e)
        {
            TappedEventArgs tap = (TappedEventArgs)e;
            Foto itemFoto = Fotos.Where(d => d.Identificador == (int)tap.Parameter).FirstOrDefault();
            if (itemFoto != null)
            {
                var Posicao = Fotos.IndexOf(itemFoto);
                lbl.Text = itemFoto.Comentario;
                cv.Position = Posicao;
            }

        }

        private void ObjTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs tap = (TappedEventArgs)e;
            Foto itemFoto = Fotos.Where(d => d.Identificador == (int)tap.Parameter).FirstOrDefault();
            if (itemFoto != null && itemFoto.Video)
            {
                Device.OpenUri(new Uri( string.Concat("https://www.youtube.com/watch?v=", itemFoto.CodigoFoto)));
            }
        }

        private async void Cv_PositionSelected(object sender, SelectedPositionChangedEventArgs e)
        {
            var ItemFoto = Fotos[(int)e.SelectedPosition];
            if (ItemFoto != null)
            {
                lbl.Text = ItemFoto.Comentario;
                Element item = listaImages[(int)e.SelectedPosition];
                await sv.ScrollToAsync(item, ScrollToPosition.Center, true);
            }
        }
    }
}
