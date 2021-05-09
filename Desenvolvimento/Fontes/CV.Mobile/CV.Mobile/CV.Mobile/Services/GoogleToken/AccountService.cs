using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Services.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace CV.Mobile.Services.GoogleToken
{
    public class AccountService : IAccountSevice
    {
        private const string BaseUrl = "https://accounts.google.com/o/oauth2/";
        private const string BaseUrlToken = "https://www.googleapis.com/oauth2/v4/";
        private readonly IApiService _apiService;

        public AccountService(IApiService apiService)
        {
            _apiService = apiService;
        }


        private HttpClient CreateHttpClient()
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (o, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(httpHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName));
            return httpClient;
        }

        CancellationTokenSource cts = null;
        public async Task AtualizarToken()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = GlobalSetting.iOSClientId;
                    redirectUri = GlobalSetting.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = GlobalSetting.AndroidClientId;
                    redirectUri = GlobalSetting.AndroidRedirectUrl;
                    break;
            }

            var account = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
            bool autenticado = false;
            if (account.Properties.ContainsKey("expires_in") && account.Properties.ContainsKey("access_date") )
            {
                DateTime data =  JsonConvert.DeserializeObject<DateTime>(account.Properties["access_date"]);
                int expiracao = Convert.ToInt32(account.Properties["expires_in"]);
                if (DateTime.UtcNow < data.AddSeconds(expiracao - 60))
                    autenticado = true;
            }
            if (!autenticado)
            {
                var authenticator = new OAuth2Authenticator(
                    clientId,
                    null,
                    GlobalSetting.Scope,
                    new Uri(GlobalSetting.AuthorizeUrl),
                    new Uri(redirectUri),
                    new Uri(GlobalSetting.AccessTokenUrl),
                    null,
                    true);
                cts = new CancellationTokenSource();
                authenticator.Completed += OnAuthCompleted;
                authenticator.Error += OnAuthError;
                await authenticator.RequestRefreshTokenAsync(account.Properties["refresh_token"]);
                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
            }
        }


        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
           
            try
            {
                var authenticator = sender as OAuth2Authenticator;
                if (authenticator != null)
                {
                    authenticator.Completed -= OnAuthCompleted;
                    authenticator.Error -= OnAuthError;
                }

                if (e.IsAuthenticated)
                {
                    var account = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
                    account.Properties["expires_in"] = e.Account.Properties["expires_in"];
                    account.Properties["access_token"] = e.Account.Properties["access_token"];
                    account.Properties["access_date"] = JsonConvert.SerializeObject(DateTime.UtcNow);
                    await SecureStorageAccountStore.SaveAsync(account, GlobalSetting.AppName);
                    
                }
               
            }
            finally
            {
                cts.Cancel();
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            cts.Cancel();

        }
    }
}
