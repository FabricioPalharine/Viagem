using CV.Mobile.Helper;
using CV.Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Fotos
{
    public class FotoService : IFoto
    {
        private HttpClient CreateHttpClient(string token = "")
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (o, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(httpHandler);

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

        public async Task UpdateMediaData( List<Foto> itemFoto)
        {
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            string AccessId = usuario.Properties["access_token"];
            using (var _httpClient = CreateHttpClient(AccessId))
            {
                string Uri = $"https://photoslibrary.googleapis.com/v1/mediaItems:batchGet?{string.Join("&", itemFoto.Select(d => string.Concat("mediaItemIds=", d.CodigoFoto)))}";

                var response = await _httpClient.GetAsync(Uri);

                var resultado = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var results = await Task.Run(() => JsonConvert.DeserializeObject<ResultadoCriacaoFoto>(resultado));
                    foreach (var item in itemFoto)
                    {
                        var result = results.mediaItemResults.Where(d => d.mediaItem.id == item.CodigoFoto).Select(d => d.mediaItem).FirstOrDefault();
                        if (result != null)
                        {
                            item.LinkFoto = result.baseUrl + (item.Video ? "=dv" : "=d");
                            item.LinkThumbnail = $"{result.baseUrl}=w240-h120";
                        }
                    }
                }
            }
        }


        public async Task UpdateMediaData(List<PontoMapa> itemFoto)
        {
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            string AccessId = usuario.Properties["access_token"];

            using (var _httpClient = CreateHttpClient(AccessId))
            {
                string Uri = $"https://photoslibrary.googleapis.com/v1/mediaItems:batchGet?{string.Join("&", itemFoto.Select(d => string.Concat("mediaItemIds=", d.GoogleId)))}";

                var response = await _httpClient.GetAsync(Uri);

                var resultado = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var results = await Task.Run(() => JsonConvert.DeserializeObject<ResultadoCriacaoFoto>(resultado));
                    foreach (var item in itemFoto)
                    {
                        var result = results.mediaItemResults.Where(d => d.mediaItem.id == item.GoogleId).Select(d => d.mediaItem).FirstOrDefault();
                        if (result != null)
                        {
                            item.UrlTumbnail = $"{result.baseUrl}=w60-h40";
                        }
                    }
                }
            }
        }

        public async Task UpdateMediaData(List<Timeline> itemFoto)
        {
            var usuario = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            string AccessId = usuario.Properties["access_token"];

            using (var _httpClient = CreateHttpClient(AccessId))
            {
                string Uri = $"https://photoslibrary.googleapis.com/v1/mediaItems:batchGet?{string.Join("&", itemFoto.Select(d => string.Concat("mediaItemIds=", d.GoogleId)))}";

                var response = await _httpClient.GetAsync(Uri);

                var resultado = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var results = await Task.Run(() => JsonConvert.DeserializeObject<ResultadoCriacaoFoto>(resultado));
                    foreach (var item in itemFoto)
                    {
                        
                            var result = results.mediaItemResults.Where(d => d.mediaItem.id == item.GoogleId).Select(d => d.mediaItem).FirstOrDefault();
                            if (result != null)
                            {
                                item.Url = result.baseUrl + (item.LinhaVideo ? "=dv" : "=d");
                                item.UrlThumbnail = $"{result.baseUrl}=w360-h240";
                            }
                        
                    }
                }
            }
        }

        public async Task SubirFoto(string AccessId, string CodigoAlbum, byte[] DadosFoto, UploadFoto itemFoto)
        {
            using (var _httpClient = CreateHttpClient(AccessId))
            {
                using (MemoryStream ms = new MemoryStream(DadosFoto))
                {
                    string Uri = String.Concat("https://photoslibrary.googleapis.com/v1/uploads");

                    System.Net.Http.StreamContent content = new StreamContent(ms);
                    content.Headers.ContentLength = DadosFoto.Length;
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(itemFoto.ImageMime);
                    content.Headers.Add("X-Goog-Upload-File-Name", itemFoto.NomeArquivo);
                    content.Headers.Add("X-Goog-Upload-Protocol", "raw");
                    var response = await _httpClient.PostAsync(Uri, content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        string UPLOAD_TOKEN = resultado;
                        List<object> dados = new List<object>();
                        dados.Add(new { description = "", simpleMediaItem = new { uploadToken = UPLOAD_TOKEN } });
                        var albumData = new { albumId = CodigoAlbum, newMediaItems = dados.ToArray() };
                        Uri = String.Concat("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate");

                        var json = JsonConvert.SerializeObject(albumData);
                        var contentjson = new StringContent(json, Encoding.UTF8, "application/json");

                        response = await _httpClient.PostAsync(Uri, contentjson);

                        resultado = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            ResultadoCriacaoFoto result = await Task.Run(() => JsonConvert.DeserializeObject<ResultadoCriacaoFoto>(resultado));
                            var item = result.newMediaItemResults.FirstOrDefault();
                            if (item != null)
                            {
                                itemFoto.LinkGoogle = itemFoto.Thumbnail = item.mediaItem.productUrl;
                                itemFoto.CodigoGoogle = item.mediaItem.id;
                                await UpdateMediaData(_httpClient, item.mediaItem.id,itemFoto);
                            }
                        }

                    }
                }
            }
        }

        private async Task UpdateMediaData(HttpClient _httpClient, string id, UploadFoto itemFoto)
        {
            string Uri = $"https://photoslibrary.googleapis.com/v1/mediaItems/{id}";

            var response = await _httpClient.GetAsync(Uri);

            var resultado = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                MediaItem result = await Task.Run(() => JsonConvert.DeserializeObject<MediaItem>(resultado));
                itemFoto.LinkGoogle = result.baseUrl + (itemFoto.Video?"=dv":"=d");
                itemFoto.Thumbnail = $"{result.baseUrl}=w360-h240";
            }
        }
    }
}
