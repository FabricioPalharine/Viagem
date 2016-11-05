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

namespace CV.Mobile.Droid.Services
{
    public class ValidarAutenticacaoService : IValidaAutenticacao
    {
        public async Task<Account> RetornarAutenticacaoAplicacao()
        {
            var accounts = await AccountStore.Create().FindAccountsForServiceAsync(Constants.AppName);
            var account = accounts.FirstOrDefault();
            return account;
        }
    }
}