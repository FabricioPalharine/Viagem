using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CV.Mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomNavigationView : NavigationPage
	{
		public CustomNavigationView ()
		{
			InitializeComponent ();
		}

        public CustomNavigationView(Page root) : base(root)
        {
            InitializeComponent();
        }
    }
}