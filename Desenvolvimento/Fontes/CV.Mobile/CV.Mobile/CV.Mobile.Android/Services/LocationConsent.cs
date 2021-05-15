using CV.Mobile.Droid.Services;
using CV.Mobile.Services.PlatformSpecifcs;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace CV.Mobile.Droid.Services
{
    public class LocationConsent : ILocationConsent
    {
        public async Task GetLocationConsent()
        {
            await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
    }
}
