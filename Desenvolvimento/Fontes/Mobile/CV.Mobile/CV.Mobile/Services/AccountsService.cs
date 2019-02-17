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
        private const string BaseUrlToken = "https://www.googleapis.com/oauth2/v4/";
        public AccountsService(): this(false)
        {

     
           

        }

        public AccountsService(bool AuthCode)
        {

            this._httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(AuthCode?BaseUrlToken: BaseUrl)
            };



        }

        public async Task<DadosGoogleToken> RetornarTokenUsuario(string authCode)
        {
            HttpResponseMessage response = null;
            DadosGoogleToken item = null;
            string Uri = String.Concat("token");


            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", Constants.ClientId),
                new KeyValuePair<string, string>("client_secret", Constants.ClientSecret),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", Constants.RedirectToURL),
            });

            response = await _httpClient.PostAsync(Uri, content);

            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject retornoCompleto = JsonConvert.DeserializeObject<JObject>(resultado);
                item = new DadosGoogleToken();
                item.expires_in  = retornoCompleto["expires_in"].ToString();
                item.access_token = retornoCompleto["access_token"].ToString();
                item.refresh_token = retornoCompleto["refresh_token"].ToString();


            }

            return item;
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
                JObject retornoCompleto = JsonConvert.DeserializeObject<JObject>(resultado);
                itemUsuario.Token = retornoCompleto["access_token"].ToString();;
                itemUsuario.Lifetime = Convert.ToInt32( retornoCompleto["expires_in"].ToString());;
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
