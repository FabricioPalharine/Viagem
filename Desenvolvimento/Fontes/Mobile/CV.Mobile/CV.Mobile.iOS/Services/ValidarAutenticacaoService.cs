using CV.Mobile.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth;
using CV.Mobile.Models;
using System.Threading.Tasks;
using Google.SignIn;

namespace CV.Mobile.iOS.Services
{
    public class ValidarAutenticacaoService : IValidaAutenticacao
    {
        public async Task Desconectar()
        {
            await Task.Delay(1);
            SignIn.SharedInstance.SignOutUser();
        }

        public async Task<Account> RetornarAutenticacaoAplicacao()
        {
            var accounts = await AccountStore.Create().FindAccountsForServiceAsync(Constants.AppName);
            var account = accounts.FirstOrDefault();
            return account;
        }
    }
}