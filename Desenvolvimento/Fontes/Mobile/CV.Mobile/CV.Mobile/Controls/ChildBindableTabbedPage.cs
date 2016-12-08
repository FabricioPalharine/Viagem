using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class ChildBindableTabbedPage : TabbedPage
    {
        public ChildBindableTabbedPage()
        {

        }


        public static BindableProperty ChildrenListProperty = BindableProperty.Create(nameof(ChildrenList), typeof(IList<Page>), typeof(ChildBindableTabbedPage), new List<Page>(),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: (bindable, oldvalue, newvalue) =>
        {
            var page = bindable as ChildBindableTabbedPage;
            var newValue = newvalue as IList<Page>;
            var oldValue = oldvalue as IList<Page>;
            if (page != null)
            {
                if (oldvalue != null)
                foreach (var item in oldValue)
                    if (!newValue.Contains(item))
                        page.Children.Remove(item);
                foreach (var item in newValue)
                    if (oldvalue == null || !oldValue.Contains(item))
                        page.Children.Add(item);
            }
        });


        public IList<Page> ChildrenList
        {
            get { return (IList<Page>)GetValue(ChildrenListProperty); }
            set { SetValue(ChildrenListProperty, value); }
        }

        private static void UpdateList(BindableObject bindable, IList<Page> oldvalue, IList<Page> newvalue)
        {

            var page = bindable as ChildBindableTabbedPage;
            if (page != null)
            {
                page.Children.Clear();
                foreach (var childpage in newvalue)
                    page.Children.Add(childpage);
            }

        }

    }

}
