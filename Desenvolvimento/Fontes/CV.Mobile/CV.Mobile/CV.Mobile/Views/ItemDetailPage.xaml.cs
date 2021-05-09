using CV.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace CV.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}