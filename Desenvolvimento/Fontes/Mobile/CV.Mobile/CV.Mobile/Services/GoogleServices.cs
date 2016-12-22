using CV.Mobile.Models;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Services
{
    public class GoogleServices: IDisposable
    {
        private static string _apiKey;


        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://maps.googleapis.com/maps/api/";
        private const string UrlPlaces = "place/nearbysearch/json?"; // ?input=SEARCHTEXT&key=API_KEY

        public GoogleServices()
        {
            _apiKey = Constants.ClientAPI;

            this._httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<GmsPlacesSearch> GetPlaces(string Texto, Position posicao, string[] tipos, int Raio )
        {
            if (Texto.Length > 0)
            {
                var result = await this._httpClient.GetAsync(this.BuildQueryPredictions(Texto, posicao, tipos, Raio));

                if (result.IsSuccessStatusCode)
                {
                    var placeResult = JsonConvert.DeserializeObject<GmsPlacesSearch>(await result.Content.ReadAsStringAsync());
                    return placeResult;
                }
            }
            return null;
        }

        private string BuildQueryPredictions(string texto, Position posicao, string[] tipos, int raio)
        {
            var cultura = new System.Globalization.CultureInfo("en-us");
            var Consulta = string.Format("{0}location={1},{2}&name={3}&radius={4}&key={5}", UrlPlaces, posicao.Latitude.ToString("F7", cultura),
                posicao.Longitude.ToString("F7",cultura), System.Net.WebUtility.UrlEncode( texto), raio.ToString("F0"), _apiKey);
            if (tipos != null && tipos.Any())
                Consulta += String.Concat("&types=", string.Join("|", tipos));
            return Consulta;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
