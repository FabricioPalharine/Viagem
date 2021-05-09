using Sharpnado.Presentation.Forms.CustomViews.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CV.Mobile.Views.Contents
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomTabTextItem : TabTextItem
    {
        public static readonly BindableProperty UnderlineAllTabProperty = BindableProperty.Create(
            nameof(UnderlineAllTab),
            typeof(bool),
            typeof(TabTextItem),
            true);

        public CustomTabTextItem()
        {
            InitializeComponent();

            LabelSize = 14;
        }

        public bool UnderlineAllTab
        {
            get => (bool)GetValue(UnderlineAllTabProperty);
            set => SetValue(UnderlineAllTabProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Margin):
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(Underline.Margin.Left - Margin.Left, 0, Underline.Margin.Right - Margin.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;

                case nameof(Padding):
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(Underline.Margin.Left - Padding.Left, 0, Underline.Margin.Right - Padding.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;

                case nameof(UnderlineAllTab):
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(-Margin.Left - Padding.Left, 0, -Margin.Right - Padding.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;
            }
        }
    }
}
