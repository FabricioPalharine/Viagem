using CV.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CV.Mobile.Views
{
    public partial class MenuPage : ContentPage
    {
        protected MenuViewModel ViewModel => BindingContext as MenuViewModel;

        public MenuPage()
        {
            InitializeComponent();
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {

            await ViewModel.ExecutarAcao();
        }

    }
}
