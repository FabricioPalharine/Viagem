using CV.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CV.Mobile.Views
{
    public partial class MasterDetailPage : Xamarin.Forms.MasterDetailPage
    {

        public MasterDetailPage(MasterDetailViewModel modelo)
        {

            this.BindingContext = modelo;
            this.Master = modelo.MasterPage;
            this.Detail = new NavigationPage( modelo.DetailPage);
            InitializeComponent();
            
        }

       
    }
}
