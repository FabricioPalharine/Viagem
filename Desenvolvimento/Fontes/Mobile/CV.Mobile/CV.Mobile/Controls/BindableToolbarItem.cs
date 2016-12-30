using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class BindableToolbarItem : ToolbarItem
    {

        public BindableToolbarItem()
        {
            InitVisibility();
        }

        private async void InitVisibility()
        {
            await Task.Delay(100);
            OnIsVisibleChanged(this, false, IsVisible);
        }

        public new Page Parent { set; get; }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static BindableProperty IsVisibleProperty = BindableProperty.Create("IsVisible", typeof(bool), typeof(BindableToolbarItem), true, BindingMode.OneWay, propertyChanged: OnIsVisibleChanged);

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as BindableToolbarItem;

            if (item.Parent == null)
                return;

            var items = ((Page)item.Parent).ToolbarItems;

            if ((bool) newvalue && !items.Contains(item))
            {
                items.Add(item);
            }
            else if (!((bool)newvalue) && items.Contains(item))
            {

                items.Remove(item);
            }
        }
    }
}
