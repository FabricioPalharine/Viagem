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
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Plus;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;

[assembly: ExportRenderer(typeof(AutenticacaoPage), typeof(AutenticacaoPageRenderer))]

namespace CV.Mobile.Droid.Renderer
{
    public class AutenticacaoPageRenderer : PageRenderer, Android.Views.View.IOnClickListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        global::Android.Views.View view;

        Activity activity;
        const int RC_SIGN_IN = 9001;

        GoogleApiClient mGoogleApiClient;

 
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            var activity = this.Context as MainActivity;

            if (e.OldElement != null)
            {
                activity.ActivityResult -= HandleActivityResult;
            }

            if (e.NewElement != null)
            {
                activity.ActivityResult += HandleActivityResult;
            }

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
                AddView(view);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
            }



        }

        private async void HandleActivityResult(object sender, ActivityResultEventArgs e)
        {
            if (e.RequestCode == 9001)
            {
                
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(e.Data);
                if (result.IsSuccess)
                {
                    GoogleSignInAccount acct = result.SignInAccount;
                    String idToken = acct.IdToken;
                    String authCode = acct.ServerAuthCode;
                    string mail = acct.Email;
                    string nome = acct.DisplayName;
                    Account conta = new Account(acct.Id);
                    DadosGoogleToken token = null;
                    using (AccountsService srv = new AccountsService(true))
                    {
                        token = await srv.RetornarTokenUsuario(authCode);
                    }
                    UsuarioLogado itemUsuario = new UsuarioLogado();

                    if (token!= null)
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
                        (App.Current as App).RedirectToMenu(itemUsuario);

                    }
                }
                else
                {
                }
            }
        }

        void SetupUserInterface()
        {
            activity = this.Context as Activity;
            
            view = activity.LayoutInflater.Inflate(Resource.Layout.login, this, false);

         }

        void SetupEventHandlers()
        {



            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
        .RequestEmail()
        .RequestProfile()
        .RequestScopes(new Scope("https://www.googleapis.com/auth/youtube.upload"), new Scope("https://www.googleapis.com/auth/youtube"), new Scope("https://www.googleapis.com/auth/photoslibrary"), new Scope("https://www.googleapis.com/auth/photoslibrary.sharing"))
        .RequestIdToken(Constants.ClientId)
        .RequestServerAuthCode(Constants.ClientId, true)

        .Build();

            mGoogleApiClient = new GoogleApiClient.Builder(this.Context)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)

                .AddScope(new Scope(Scopes.Profile))
                .AddScope(new Scope("https://www.googleapis.com/auth/youtube.upload"))
                .AddScope(new Scope("https://www.googleapis.com/auth/youtube"))
                .AddScope( new Scope("https://www.googleapis.com/auth/photoslibrary"))
                .AddScope( new Scope("https://www.googleapis.com/auth/photoslibrary.sharing"))
                .Build();

            Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            // Android.App.Activity.StartActivityForResult(signInIntent, RC_SIGN_IN);;
            activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
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

        //void UpdateUI(bool isSignedIn)
        //{
        //    Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient);

        //    Auth.GoogleSignInApi.SignOut(mGoogleApiClient);
        //}

        //protected override void OnStart()
        //{
        //    base.OnStart();
        //    mGoogleApiClient.Connect();
        //}

        //protected override void OnStop()
        //{
        //    base.OnStop();
        //    mGoogleApiClient.Disconnect();
        //}

        //protected override void OnSaveInstanceState(Bundle outState)
        //{
        //    base.OnSaveInstanceState(outState);
        //    outState.PutBoolean(KEY_IS_RESOLVING, mIsResolving);
        //    outState.PutBoolean(KEY_SHOULD_RESOLVE, mIsResolving);
        //}

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //    //Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

        //    if (requestCode == RC_SIGN_IN)
        //    {
        //        if (resultCode != Result.Ok)
        //        {
        //            mShouldResolve = false;
        //        }

        //        mIsResolving = false;
        //        mGoogleApiClient.Connect();
        //    }
        //}

        public void OnConnected(Bundle connectionHint)
        {
            ///Log.Debug(TAG, "onConnected:" + connectionHint);
           // var intent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            //GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(intent);
            //if (result.IsSuccess)
            //{
            //    GoogleSignInAccount acct = result.SignInAccount;
            //    String idToken = acct.IdToken;
            //    String authCode = acct.ServerAuthCode;
            //    string mail = acct.Email;
            //    string nome = acct.DisplayName;
            //}
            //    UpdateUI(true);
        }

        public void OnConnectionSuspended(int cause)
        {
            //Log.Warn(TAG, "onConnectionSuspended:" + cause);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
           // Log.Debug(TAG, "onConnectionFailed:" + result);

            
            
            
        }

        class DialogInterfaceOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
        {
            public Action<IDialogInterface> OnCancelImpl { get; set; }

            public void OnCancel(IDialogInterface dialog)
            {
                OnCancelImpl(dialog);
            }
        }

        void ShowErrorDialog(ConnectionResult connectionResult)
        {
            int errorCode = connectionResult.ErrorCode;


        }

        public  void OnClick(Android.Views.View v)
        {
            //mGoogleApiClient.Connect();

            //await Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient);

          //  await Auth.GoogleSignInApi.SignOut(mGoogleApiClient);
           
            // mStatus.Text = GetString(Resource.String.signing_in);
          //  mShouldResolve = true;
           //         mGoogleApiClient.Connect();
                
            
        }
    }
}