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
    public class AccountsService : IDisposable
    {


        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://accounts.google.com/o/oauth2/";

        public AccountsService()
        {

            this._httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(BaseUrl)
            };

           

        }

        public async Task AtualizarTokenUsuario(Usuario itemUsuario)
        {
      
            string Uri = String.Concat("token");
            HttpResponseMessage response = null;



            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", Constants.ClientId),
                new KeyValuePair<string, string>("client_secret", Constants.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", itemUsuario.RefreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),

            });

            response = await _httpClient.PostAsync(Uri, content);

            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var retornoCompleto = JsonConvert.DeserializeObject<dynamic>(resultado);
                itemUsuario.Token = retornoCompleto.access_token;
                itemUsuario.Lifetime = retornoCompleto.expires_in;
                itemUsuario.DataToken = DateTime.Now.ToUniversalTime();
                using (ApiService srv = new ApiService())
                {
                    await srv.SalvarUsuario(itemUsuario);
                }
            }
            
        }

     
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
