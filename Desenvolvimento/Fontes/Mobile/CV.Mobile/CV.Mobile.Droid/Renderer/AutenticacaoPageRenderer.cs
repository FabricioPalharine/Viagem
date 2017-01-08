using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using CV.Mobile.Views;
using CV.Mobile.Droid.Renderer;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using CV.Mobile.Models;
using CV.Mobile.Services;

[assembly: ExportRenderer(typeof(AutenticacaoPage), typeof(AutenticacaoPageRenderer))]

namespace CV.Mobile.Droid.Renderer
{
    public class AutenticacaoPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

          

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
                    var activity = Context as Activity;
                    activity.StartActivity(auth.GetUI(activity));
                
          
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
                if (itemUsuario.Codigo > 0)
                   await  (App.Current as App).GravarUsuario(itemUsuario.Codigo);
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
            (App.Current as App).RedirectToMenu(itemUsuario);
        }
    }
}