using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CV.Mobile.Helpers;

namespace CV.Mobile.Services
{
    public class ApiService : IDisposable
    {
        HttpClient client;
        public string AuthenticationToken {
            set
            {
                if (client.DefaultRequestHeaders.Contains("Authorization"))
                    client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", String.Concat("Bearer ", value));
            }
        }

        public ApiService()
        {
            client = new HttpClient();
            var secureSettings = Plugin.SecureStorage.CrossSecureStorage.Current;
            if (secureSettings.HasKey("AuthenticationToken"))
            {
                if (client.DefaultRequestHeaders.Contains("Authorization"))
                    client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", String.Concat("Bearer ", secureSettings.GetValue("AuthenticationToken")));
            }

        }

        public ApiService(string authenticationToken)
        {
            client = new HttpClient();
            AuthenticationToken = authenticationToken;

        }
        public void Dispose()
        {
            client.Dispose();
            client = null;
        }

        public async Task<UsuarioLogado> LogarUsuario(DadosGoogleToken DadosLogin)
        {
            UsuarioLogado itemUsuario = new UsuarioLogado();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Acesso/LoginAplicativo"));
            var json = JsonConvert.SerializeObject(DadosLogin);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemUsuario = JsonConvert.DeserializeObject<UsuarioLogado>(resultado);
                AuthenticationToken = itemUsuario.AuthenticationToken;
                var secureSettings = Plugin.SecureStorage.CrossSecureStorage.Current;

                secureSettings.SetValue("AuthenticationToken", itemUsuario.AuthenticationToken);
            }


            return itemUsuario;
        }

        public async Task<UsuarioLogado> CarregarDadosAplicativo(UsuarioLogado DadosLogin)
        {
            UsuarioLogado itemUsuario = new UsuarioLogado();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Acesso/CarregarDadosAplicativo"));
            var json = JsonConvert.SerializeObject(DadosLogin);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemUsuario = JsonConvert.DeserializeObject<UsuarioLogado>(resultado);
                AuthenticationToken = itemUsuario.AuthenticationToken;
                var secureSettings = Plugin.SecureStorage.CrossSecureStorage.Current;

                secureSettings.SetValue("AuthenticationToken", itemUsuario.AuthenticationToken);
            }


            return itemUsuario;
        }
    }
}
