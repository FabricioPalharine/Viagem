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
            string Uri = String.Concat("https://photoslibrary.googleapis.com/v1/uploads");
            HttpResponseMessage response = null;


            using (MemoryStream ms = new MemoryStream(Imagem))
            {
                System.Net.Http.StreamContent content = new StreamContent(ms);
                content.Headers.ContentLength = Imagem.Length;
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(itemUpload.ImageMime);
                content.Headers.Add("X-Goog-Upload-File-Name", itemUpload.NomeArquivo);
                content.Headers.Add("X-Goog-Upload-Protocol", "raw");
                response = await _httpClient.PostAsync(Uri, content);

                var resultado = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    string UPLOAD_TOKEN = resultado;

                    List<object> dados = new List<object>();
                    dados.Add(new { description = "", simpleMediaItem = new { uploadToken = UPLOAD_TOKEN } });
                    var albumData = new { albumId = AlbumId, newMediaItems = dados.ToArray() };
                    Uri = String.Concat("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate");

                    var json = JsonConvert.SerializeObject(albumData);
                    var contentjson = new StringContent(json, Encoding.UTF8, "application/json");

                    response = await _httpClient.PostAsync(Uri, contentjson);

                    resultado = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic resultadoDinamico = JObject.Parse(resultado);
                        itemUpload.LinkGoogle = itemUpload.Thumbnail = resultadoDinamico.newMediaItemResults[0].mediaItem.productUrl;
                        itemUpload.CodigoGoogle = resultadoDinamico.newMediaItemResults[0].mediaItem.id;
                    }

                }
            }
        }

     
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
