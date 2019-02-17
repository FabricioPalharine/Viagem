using CV.Mobile.iOS.Renderer;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using Foundation;
using Google.SignIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AutenticacaoPage), typeof(AutenticacaoPageRenderer))]

namespace CV.Mobile.iOS.Renderer
{
    public class AutenticacaoPageRenderer: PageRenderer, ISignInUIDelegate, ISignInDelegate
    {
        
        public AutenticacaoPageRenderer()
        {
            SignIn.SharedInstance.UIDelegate = this;
            SignIn.SharedInstance.Delegate = this;
            SignIn.SharedInstance.Scopes = new string[] {                 "profile",
                "https://www.googleapis.com/auth/youtube.upload",
                "https://www.googleapis.com/auth/youtube",
                "https://www.googleapis.com/auth/photoslibrary",
               "https://www.googleapis.com/auth/photoslibrary.sharing" };
            SignIn.SharedInstance.ClientID = "997990659234-uqkc1c02tkiv6sl5atvf6it6b1u986kq.apps.googleusercontent.com";
            SignIn.SharedInstance.ServerClientID = Constants.ClientId;
        }

        //public override void ViewDidAppear(bool animated)
        //{
        //    base.ViewDidAppear(animated);

        //          // Initialize the object that communicates with the OAuth service
        //            var auth = new OAuth2AuthenticatorToken(
        //                           Constants.ClientId,
        //                           Constants.ClientSecret,
        //                           Constants.Scopes,
        //                           new Uri(Constants.AuthorizeUrl),
        //                           new Uri(Constants.RedirectToURL),
        //                           new Uri(Constants.AccessTokenUrl));

        //            // Register an event handler for when the authentication process completes
        //            auth.Completed += OnAuthenticationCompleted;

        //            // Display the UI
        //            PresentViewController(auth.GetUI(), true, null);

        //}

        //async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        //{
        //    UsuarioLogado itemUsuario = new UsuarioLogado();
        //    if (e.IsAuthenticated)
        //    {
        //        DadosGoogleToken token = new DadosGoogleToken();
        //        token.access_token = e.Account.Properties["access_token"];
        //        token.expires_in = e.Account.Properties["expires_in"];
        //        token.refresh_token = e.Account.Properties["refresh_token"];
        //        using (ApiService srv = new ApiService())
        //        {
        //            itemUsuario = await srv.LogarUsuario(token);
        //        }
        //        if (itemUsuario.Codigo > 0)
        //            await (App.Current as App).GravarUsuario(itemUsuario.Codigo);
        //        if (!string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
        //        {
        //            e.Account.Username = itemUsuario.CodigoGoogle;
        //            e.Account.Properties.Add("Codigo", itemUsuario.Codigo.ToString());
        //            e.Account.Properties.Add("Nome", itemUsuario.Nome);
        //            e.Account.Properties.Add("LinkFoto", itemUsuario.LinkFoto);
        //            e.Account.Properties.Add("Email", itemUsuario.Email);
        //            e.Account.Properties.Add("AuthenticationToken", itemUsuario.AuthenticationToken);
        //            AccountStore.Create().Save(e.Account, Constants.AppName);
        //        }
        //    }
        //    // If the user is logged in navigate to the TodoList page.
        //    // Otherwise allow another login attempt.
        //    (App.Current as App).RedirectToMenu(itemUsuario);
        //}

        TaskCompletionSource<string> _taskCompletionSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SignIn.SharedInstance.SignOutUser();
            SignIn.SharedInstance.SignInUser();
        }


        public Task<string> GetAccessToken()
        {
            _taskCompletionSource = new TaskCompletionSource<string>();
            SignIn.SharedInstance.SignInUser();
            return _taskCompletionSource.Task;
        }

        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            if (error != null)
            {
                _taskCompletionSource.SetException(new NSErrorException(error));
            }
            else
            {
                Task<UsuarioLogado> task =Task<UsuarioLogado>.Run(async () => await RedirecionarUsuario(user));
                UsuarioLogado resultado = task.Result;
                if (resultado.Codigo>0)
                (App.Current as App).RedirectToMenu(resultado);

            }
        }
        
        public async Task<UsuarioLogado> RedirecionarUsuario(GoogleUser user)
        {
            UsuarioLogado itemUsuario = new UsuarioLogado();
            String idToken = user.Authentication.IdToken;
            String authCode = user.ServerAuthCode;
            try
            {
                Account conta = new Account(user.UserID);
                DadosGoogleToken token = null;
                using (AccountsService srv = new AccountsService(true))
                {
                    token = await srv.RetornarTokenUsuario(authCode);
                }

                if (token != null)
                {
                    using (ApiService srv = new ApiService())
                    {
                        itemUsuario = await srv.LogarUsuario(token);
                    }
                    if (itemUsuario.Codigo > 0)
                        await (App.Current as App).GravarUsuario(itemUsuario.Codigo);
                    if (!string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
                    {
                        conta.Username = itemUsuario.CodigoGoogle;
                        conta.Properties.Add("Codigo", itemUsuario.Codigo.ToString());
                        conta.Properties.Add("Nome", itemUsuario.Nome);
                        conta.Properties.Add("LinkFoto", itemUsuario.LinkFoto);
                        conta.Properties.Add("Email", itemUsuario.Email);
                        conta.Properties.Add("AuthenticationToken", itemUsuario.AuthenticationToken);
                        AccountStore.Create().Save(conta, Constants.AppName);
                    }

                }
            }
            catch (Exception ex)
            {
                string erro = ex.Message;

            }
            return itemUsuario;
        }
    }

   
}