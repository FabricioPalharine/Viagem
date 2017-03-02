using CV.Mobile.Models;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Services
{
    public class PhotosService : IDisposable
    {
        private static string _apiKey;


        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://picasaweb.google.com/data/feed/api/";
        private const string UrlPlaces = "place/nearbysearch/json?"; // ?input=SEARCHTEXT&key=API_KEY

        public PhotosService(string TokenUsuario)
        {
            _apiKey = Constants.ClientAPI;

            this._httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(BaseUrl)
            };

            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", String.Concat("Bearer ", TokenUsuario));

        }

        public async Task SubirFoto(string AlbumId, byte[] Imagem, UploadFoto itemUpload)
        {

            string Uri = String.Concat("user/default/albumid/default", "?alt=json-in-script");
            HttpResponseMessage response = null;
           

            using (MemoryStream ms = new MemoryStream(Imagem))
            {
                System.Net.Http.StreamContent content = new StreamContent(ms);
                content.Headers.ContentLength = Imagem.Length;
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(itemUpload.ImageMime);
                response = await _httpClient.PostAsync(Uri, content);

                var resultado = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    resultado = resultado.Substring(0, resultado.Length - 2).Replace("gdata.io.handleScriptLoaded(", "");

                    var newimageinfo = JsonConvert.DeserializeObject<dynamic>(resultado);

                    itemUpload.CodigoGoogle = ((Newtonsoft.Json.Linq.JContainer)(newimageinfo.entry["gphoto$id"])).First.First.ToString();


                    itemUpload.LinkGoogle = newimageinfo.entry["media$group"]["media$content"][0]["url"].ToString();
                    itemUpload.Thumbnail = newimageinfo.entry["media$group"]["media$thumbnail"][1]["url"].ToString();

                }
            }
        }

     
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
