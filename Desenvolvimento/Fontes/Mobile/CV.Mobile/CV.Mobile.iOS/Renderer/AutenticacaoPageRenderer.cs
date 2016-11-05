using CV.Mobile.iOS.Renderer;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AutenticacaoPage), typeof(AutenticacaoPageRenderer))]

namespace CV.Mobile.iOS.Renderer
{
    public class AutenticacaoPageRenderer: PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

                  // Initialize the object that communicates with the OAuth service
                    var auth = new OAuth2AuthenticatorToken(
                                   Constants.ClientId,
                                   Constants.ClientSecret,
                                   Constants.Scopes,
                                   new Uri(Constants.AuthorizeUrl),
                                   new Uri(Constants.RedirectToURL),
                                   new Uri(Constants.AccessTokenUrl));

                    // Register an event handler for when the authentication process completes
                    auth.Completed += OnAuthenticationCompleted;

                    // Display the UI
                    PresentViewController(auth.GetUI(), true, null);
               
        }

        async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            UsuarioLogado itemUsuario = new UsuarioLogado();
            if (e.IsAuthenticated)
            {
                DadosGoogleToken token = new DadosGoogleToken();
                token.access_token = e.Account.Properties["access_token"];
                token.expires_in = e.Account.Properties["expires_in"];
                token.refresh_token = e.Account.Properties["refresh_token"];
                using (ApiService srv = new ApiService())
                {
                    itemUsuario = await srv.LogarUsuario(token);
                }
                if (!string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
                {
                    e.Account.Username = itemUsuario.CodigoGoogle;
                    e.Account.Properties.Add("Codigo", itemUsuario.Codigo.ToString());
                    e.Account.Properties.Add("Nome", itemUsuario.Nome);
                    e.Account.Properties.Add("LinkFoto", itemUsuario.LinkFoto);
                    e.Account.Properties.Add("Email", itemUsuario.Email);
                    e.Account.Properties.Add("AuthenticationToken", itemUsuario.AuthenticationToken);
                    AccountStore.Create().Save(e.Account, Constants.AppName);
                }
            }
            // If the user is logged in navigate to the TodoList page.
            // Otherwise allow another login attempt.
            (App.Current as App).RedirectToMenu(itemUsuario);
        }

    }
}