using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CV.Mobile.Views.Amigos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmigoListaPage : ContentView
    {
        public AmigoListaPage()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty VisivelProperty = BindableProperty.Create(nameof(Visivel), typeof(bool), typeof(AmigoListaPage), false, BindingMode.OneWay, null,  new BindableProperty.BindingPropertyChangedDelegate(OnValueChanged));

        public event EventHandler PageVisibleChanged;

        public bool Visivel
        {
            get { return (bool)GetValue(VisivelProperty); }
            set { SetValue(VisivelProperty, value); }
        }

       

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            AmigoListaPage element = (AmigoListaPage)bindable;


            if (newValue is bool && ((bool) newValue))
            if (element.PageVisibleChanged != null)
                element.PageVisibleChanged(element, new EventArgs());

        }
    }
}