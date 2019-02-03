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
using CV.Mobile.Interfaces;
using Xamarin.Auth;
using CV.Mobile.Models;
using System.Threading.Tasks;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Common;

namespace CV.Mobile.Droid.Services
{
    public class ValidarAutenticacaoService : IValidaAutenticacao
    {
        public async Task Desconectar()
        {
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
        .RequestEmail()
        .RequestProfile()
        .RequestScopes( new Scope("https://www.googleapis.com/auth/youtube.upload"), new Scope("https://www.googleapis.com/auth/youtube"), new Scope("https://www.googleapis.com/auth/photoslibrary"), new Scope("https://www.googleapis.com/auth/photoslibrary.sharing"))
        .RequestIdToken(Constants.ClientId)
        .RequestServerAuthCode(Constants.ClientId, true)

        .Build();

            var mGoogleApiClient = new GoogleApiClient.Builder(Android.App.Application.Context)
              
                .AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)

                .AddScope(new Scope(Scopes.Profile))
                .AddScope(new Scope("https://www.googleapis.com/auth/youtube.upload"))
                .AddScope(new Scope("https://www.googleapis.com/auth/youtube"))
                                .AddScope(new Scope("https://www.googleapis.com/auth/photoslibrary"))
                .AddScope(new Scope("https://www.googleapis.com/auth/photoslibrary.sharing"))
                .Build();
            mGoogleApiClient.Connect();
            await Task.Delay(5000);
            if (mGoogleApiClient.IsConnected)
             await Auth.GoogleSignInApi.SignOut(mGoogleApiClient);
        }

        public async Task<Account> RetornarAutenticacaoAplicacao()
        {
            var accounts = await AccountStore.Create().FindAccountsForServiceAsync(Constants.AppName);
            var account = accounts.FirstOrDefault();
            return account;
        }
    }
}