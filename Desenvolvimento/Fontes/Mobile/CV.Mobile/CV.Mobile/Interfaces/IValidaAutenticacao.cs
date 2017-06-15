using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace CV.Mobile.Interfaces
{
    public interface IValidaAutenticacao
    {
        Task<Account> RetornarAutenticacaoAplicacao();
        Task Desconectar();
    }
}
