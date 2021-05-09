using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CV.Mobile.Resources;
using CV.Mobile.Services.Dialog;
using CV.Mobile.Services.Navigation;
using CV.Mobile.ViewModels;

namespace CV.Mobile.Services.RequestProvider
{
    public class RequestProviderService : IRequestProvider
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private string UniqueToken = null;
        public RequestProviderService(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            //_serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
        }

        public async Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);

        

            var content = new StringContent(JsonConvert.SerializeObject(data, _serializerSettings));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
        }

        public async Task PostNullAsync<TResult>(string uri, TResult data, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);



            var content = new StringContent(JsonConvert.SerializeObject(data, _serializerSettings));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);

            await HandleResponse(response);
         
        }

        public async Task<TReturn> PostAsync<TResult,TReturn>(string uri, TResult data, string token = "", int? timeout = null)
        {
            HttpClient httpClient = CreateHttpClient(token);
            if (timeout.HasValue)
                httpClient.Timeout = TimeSpan.FromSeconds(timeout.Value);



            var content = new StringContent(JsonConvert.SerializeObject(data, _serializerSettings));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TReturn result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TReturn>(serialized, _serializerSettings));

            return result;
        }

        /* public async Task<TResult> PostAsync<TResult>(string uri, string data, string clientId, string clientSecret)
         {
             HttpClient httpClient = CreateHttpClient(string.Empty);

             if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
             {
                 AddBasicAuthenticationHeader(httpClient, clientId, clientSecret);
             }

             var content = new StringContent(data);
             content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
             HttpResponseMessage response = await httpClient.PostAsync(uri, content);

             await HandleResponse(response);
             string serialized = await response.Content.ReadAsStringAsync();

             TResult result = await Task.Run(() =>
                 JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

             return result;
         }*/

        public async Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);

           
            var content = new StringContent(JsonConvert.SerializeObject(data, _serializerSettings));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PutAsync(uri, content);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
        }

        public async Task<TResult> DeleteAsync<TResult>(string uri, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
            HttpResponseMessage response =  await httpClient.DeleteAsync(uri);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
            return result;

        }

        private HttpClient CreateHttpClient(string token = "")
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (o, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(httpHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName));
            return httpClient;
        }

        /*private void AddHeaderParameter(HttpClient httpClient, string parameter)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrEmpty(parameter))
                return;

            httpClient.DefaultRequestHeaders.Add(parameter, Guid.NewGuid().ToString());
        }*/

        /*private void AddBasicAuthenticationHeader(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                return;

            httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
        }*/

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (response.Headers.Contains("NewToken") && response.Headers.TryGetValues("NewToken", out var valuesToken) && valuesToken.Any())
            {
                GlobalSetting.Instance.AuthToken = valuesToken.FirstOrDefault();
            }
            if (response.Headers.Contains("uniqueoperationtoken") && response.Headers.TryGetValues("uniqueoperationtoken", out  valuesToken) && valuesToken.Any())
            {
                UniqueToken= valuesToken.FirstOrDefault();
            }
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _dialogService.ShowAlertAsync(AppResource.MensagemAutenticacao, AppResource.AppName, AppResource.Continuar);
                    await _navigationService.TrocarPaginaShell("//LoginPage");
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        public string GetUniqueToken()
        {
            return UniqueToken;
        }

        public void SetUniqueToken(string Token)
        {
            UniqueToken = Token;
        }
    }
}