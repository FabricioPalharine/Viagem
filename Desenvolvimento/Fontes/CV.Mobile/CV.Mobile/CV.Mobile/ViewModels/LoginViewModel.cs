using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Services.Api;
using CV.Mobile.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Essentials;
using CV.Mobile.Services.GoogleToken;

namespace CV.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        Account account;
        public Command LoginCommand { get; }
		private readonly IApiService _apiService;
		private readonly IAccountSevice _accountSevice;
		private ImageSource _imageFundo = null;
		private bool _exibirBotao = false;
		public LoginViewModel(IApiService apiService, IAccountSevice accountSevice)
        {
			_apiService = apiService;
			_accountSevice = accountSevice;
            LoginCommand = new Command(OnLoginClicked);
			ImageFundo = ImageSource.FromResource("CV.Mobile.Resources.TelaInicial.jpg");
        }

		public ImageSource ImageFundo
        {
			get { return _imageFundo; }
			set { SetProperty(ref _imageFundo, value); }
        }

		public bool ExibirBotao
        {
			get { return _exibirBotao; }
			set { SetProperty(ref _exibirBotao, value); }
        }

		public Command PageAppearingCommand
		{
			get
			{
				return new Command( async () => 
				{
					IsBusy = true;
					try
					{
						UsuarioLogado itemUsuario = null;

						var autenticacao = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();
						if (autenticacao != null && autenticacao.Properties.ContainsKey("refresh_token"))
						{

							GlobalSetting.Instance.AuthToken = autenticacao.Properties["AuthenticationToken"];

							var current = Connectivity.NetworkAccess;
							if (current != NetworkAccess.None && current != NetworkAccess.Unknown)
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

								account = (await SecureStorageAccountStore.FindAccountsForServiceAsync(GlobalSetting.AppName)).FirstOrDefault();

								var authenticator = new OAuth2Authenticator(
									clientId,
									null,
									GlobalSetting.Scope,
									new Uri(GlobalSetting.AuthorizeUrl),
									new Uri(redirectUri),
									new Uri(GlobalSetting.AccessTokenUrl),
									null,
									true);

								authenticator.Completed += OnAuthCompleted;
								authenticator.Error += OnAuthError;
								await authenticator.RequestRefreshTokenAsync(account.Properties["refresh_token"]);

							}
							else
							{
								itemUsuario = new UsuarioLogado();
								GlobalSetting.Instance.BaseEndpoint =  itemUsuario.AuthenticationToken = await SecureStorage.GetAsync("AuthenticationToken");
								itemUsuario.Codigo = Convert.ToInt32(autenticacao.Properties["Codigo"]);
								itemUsuario.CodigoGoogle = autenticacao.Username;
								itemUsuario.Email = autenticacao.Properties["Email"];
								itemUsuario.LinkFoto = autenticacao.Properties["LinkFoto"];
								itemUsuario.Nome = autenticacao.Properties["Nome"];
							}
							if (itemUsuario != null && !string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
							{
								await AbrirAplicacao(itemUsuario);

							}
						}
						else
							ExibirBotao = true;
						
						

					}
					finally
					{
						IsBusy = false;
					}
				});
			}
		}

        private void OnLoginClicked(object obj)
        {
			string clientId = null;
			string redirectUri = null;
			ExibirBotao = false;
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

			account = null;
			
			var authenticator = new OAuth2Authenticator(
				clientId,
				null,
				GlobalSetting.Scope,
				new Uri(GlobalSetting.AuthorizeUrl),
				new Uri(redirectUri),
				new Uri(GlobalSetting.AccessTokenUrl),
				null,
				true);

			authenticator.Completed += OnAuthCompleted;
			authenticator.Error += OnAuthError;

			AuthenticationState.Authenticator = authenticator;

			var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			presenter.Login(authenticator);
		}

		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			IsBusy = true;
			try
			{
				var authenticator = sender as OAuth2Authenticator;
				if (authenticator != null)
				{
					authenticator.Completed -= OnAuthCompleted;
					authenticator.Error -= OnAuthError;
				}

				User user = null;
				if (e.IsAuthenticated)
				{
					// If the user is authenticated, request their basic user data from Google
					// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
					var request = new OAuth2Request("GET", new Uri(GlobalSetting.UserInfoUrl), null, e.Account);
					var response = await request.GetResponseAsync();
					if (response != null)
					{
						// Deserialize the data and store it in the account store
						// The users email address will be used to identify data in SimpleDB
						string userJson = await response.GetResponseTextAsync();
						user = JsonConvert.DeserializeObject<User>(userJson);
					}

					if (user != null)
					{
						DadosGoogleToken token = new DadosGoogleToken()
						{
							refresh_token = e.Account.Properties.ContainsKey("refresh_token") ? e.Account.Properties["refresh_token"] : null,
							expires_in = e.Account.Properties["expires_in"],
							access_token = e.Account.Properties.ContainsKey("access_token") ? e.Account.Properties["access_token"] : null,
							Nome = user.Name,
							CodigoGoogle=user.Id,
							EMail=user.Email
						};
						if (token != null)
						{
							UsuarioLogado itemUsuario = await _apiService.LogarUsuario(token);
							if (itemUsuario != null && !string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
							{
								itemUsuario.LinkFoto = user.Picture;
								if (account == null)
								{
									e.Account.Username = itemUsuario.CodigoGoogle;
									e.Account.Properties.Add("Codigo", itemUsuario.Codigo.ToString());
									e.Account.Properties.Add("Nome", user.Name);
									e.Account.Properties.Add("LinkFoto", user.Picture);
									e.Account.Properties.Add("Email", user.Email);
									e.Account.Properties.Add("AuthenticationToken", itemUsuario.AuthenticationToken);
									e.Account.Properties.Add("access_date", JsonConvert.SerializeObject(DateTime.UtcNow));
									await SecureStorage.SetAsync("AuthenticationToken", itemUsuario.AuthenticationToken);

									account = e.Account;
								}
								else
								{
									account.Properties["expires_in"] = e.Account.Properties["expires_in"];
									account.Properties["access_token"] = e.Account.Properties["access_token"];
									account.Properties["access_date"] = JsonConvert.SerializeObject(DateTime.UtcNow);
								}


								await SecureStorageAccountStore.SaveAsync( account, GlobalSetting.AppName);

								await AbrirAplicacao(itemUsuario);

							}
						}
						else
							ExibirBotao = true;
					}

				}
			}
			finally
            {
				IsBusy = false;
            }
		}

		private async Task AbrirAplicacao(UsuarioLogado usuarioLogado)
		{
			await Task.Delay(1);
			GlobalSetting.Instance.UsuarioLogado = usuarioLogado;
			Device.BeginInvokeOnMainThread(async () =>
			await NavigationService.IniciarNavegacao<AppShellViewModel>(usuarioLogado));

		}

		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}
			ExibirBotao = true;

			Debug.WriteLine("Authentication error: " + e.Message);
		}
	}
}
