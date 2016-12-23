using CV.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class StarControl: StackLayout
    {
        public static readonly BindableProperty NotaProperty = BindableProperty.Create
            (nameof(Nota), typeof(int?), typeof(StarControl), default(int?));

        public int? Nota
        {
            get { return (int?)this.GetValue(NotaProperty); }
            set { this.SetValue(NotaProperty, value); OnPropertyChanged("Nota"); }
        }

        public StarControl()
        {
            this.Init();
        }

        private async void Init()
        {
            await System.Threading.Tasks.Task.Delay(500);

            TapGestureRecognizer objTapGestureRecognizer = new TapGestureRecognizer();
            objTapGestureRecognizer.CommandParameter = 1;
            objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
            Image img = new Image()
            {
                BindingContext = this
            };
            img.GestureRecognizers.Add(objTapGestureRecognizer);
            img.SetBinding(Image.SourceProperty, new Binding("Nota", converter: new IntegerImageSourceConverter(), converterParameter: 1));
            this.Children.Add(img);
            objTapGestureRecognizer = new TapGestureRecognizer();
            objTapGestureRecognizer.CommandParameter = 2;
            objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
            img = new Image()
            {
                BindingContext = this
            };
            img.GestureRecognizers.Add(objTapGestureRecognizer);
            img.SetBinding(Image.SourceProperty, new Binding("Nota", converter: new IntegerImageSourceConverter(), converterParameter: 2));
            this.Children.Add(img);
            objTapGestureRecognizer = new TapGestureRecognizer();
            objTapGestureRecognizer.CommandParameter = 3;
            objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
            img = new Image()
            {
                BindingContext = this
            };
            img.GestureRecognizers.Add(objTapGestureRecognizer);
            img.SetBinding(Image.SourceProperty, new Binding("Nota", converter: new IntegerImageSourceConverter(), converterParameter: 3));
            this.Children.Add(img);
            objTapGestureRecognizer = new TapGestureRecognizer();
            objTapGestureRecognizer.CommandParameter = 4;
            objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
            img = new Image()
            {
                BindingContext = this
            };
            img.GestureRecognizers.Add(objTapGestureRecognizer);
            img.SetBinding(Image.SourceProperty, new Binding("Nota", converter: new IntegerImageSourceConverter(), converterParameter: 4));
            this.Children.Add(img);
            objTapGestureRecognizer = new TapGestureRecognizer();
            objTapGestureRecognizer.CommandParameter = 5;
            objTapGestureRecognizer.Tapped += ObjTapGestureRecognizer_Tapped;
            img = new Image()
            {
                BindingContext = this
            };
            img.GestureRecognizers.Add(objTapGestureRecognizer);
            img.SetBinding(Image.SourceProperty, new Binding("Nota", converter: new IntegerImageSourceConverter(), converterParameter: 5));
            this.Children.Add(img);
        }

        private void ObjTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs tap = (TappedEventArgs)e;
            Nota = Convert.ToInt32(tap.Parameter);
        }
    }
}
