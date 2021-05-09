
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Controls
{
    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
