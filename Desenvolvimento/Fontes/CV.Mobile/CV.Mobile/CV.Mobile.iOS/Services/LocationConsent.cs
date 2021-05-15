using CoreLocation;
using System.Threading.Tasks;
using UIKit;
using CV.Mobile.iOS.Services;
using CV.Mobile.Services.PlatformSpecifcs;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]

namespace CV.Mobile.iOS.Services
{
    public class LocationConsent : ILocationConsent
    {
        public static LocationManager Manager { get; set; }
        public LocationConsent()
        {
            Manager = new LocationManager();
            Manager.StartLocationUpdates();
        }
        public Task GetLocationConsent()
        {
            var manager = new CLLocationManager();
            manager.AuthorizationChanged += (sender, args) => {
                //Console.WriteLine("Authorization changed to: {0}", args.Status);
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                manager.RequestAlwaysAuthorization();
            }
            return Task.FromResult(true);
        }
    }
}
