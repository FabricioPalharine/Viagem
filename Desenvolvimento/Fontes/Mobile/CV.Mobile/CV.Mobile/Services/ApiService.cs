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
        public string AuthenticationToken
        {
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

        public async Task<List<Viagem>> ListarViagens(CriterioBusca criterioBusca)
        {
            var ListaViagem = new List<Viagem>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Viagem>>(resultado);
                ListaViagem = itemResultado.Lista;
            }

            return ListaViagem;
        }

        public async Task<List<Usuario>> ListarAmigos()
        {
            var ListaAmigos = new List<Usuario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Usuario/listaAmigos"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
                ListaAmigos = itemResultado;
            }

            return ListaAmigos;
        }

        public async Task<List<Usuario>> ListarAmigosComigo()
        {
            var ListaAmigos = new List<Usuario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Usuario/listaAmigosComigo"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
                ListaAmigos = itemResultado;
            }

            return ListaAmigos;
        }

        public async Task<UsuarioLogado> SelecionarViagem(int? IdentificadorViagem)
        {
            PocoLogin itemLogin = new PocoLogin();
            itemLogin.IdentificadorViagem = IdentificadorViagem;
            UsuarioLogado itemUsuario = new UsuarioLogado();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Acesso/SelecionarViagem"));
            var json = JsonConvert.SerializeObject(itemLogin);
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

        public async Task<Viagem> CarregarViagem(int? IdentificadorViagem)
        {
            Viagem itemViagem = new Viagem();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/Get/", IdentificadorViagem));

            HttpResponseMessage response = null;
            response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemViagem = JsonConvert.DeserializeObject<Viagem>(resultado);
            }


            return itemViagem;
        }

        public async Task<ResultadoOperacao> SalvarViagem(Viagem itemViagem)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/Post"));
            var json = JsonConvert.SerializeObject(itemViagem, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }


        public async Task<Boolean> VerificarOnLine()
        {
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Acesso/VerificaOnline"));
            bool retorno = false;
            HttpResponseMessage response = null;
            response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                retorno = JsonConvert.DeserializeObject<bool>(resultado);
            }


            return retorno;
        }

        public async Task<List<ConsultaAmigo>> ListarAmigosConsulta()
        {
            var ListaAmigos = new List<ConsultaAmigo>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Amigo/get?json={}"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<ConsultaAmigo>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<List<RequisicaoAmizade>> ListarRequisicaoAmizade()
        {
            var ListaAmigos = new List<RequisicaoAmizade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/RequisicaoAmizade/get?json={}"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<RequisicaoAmizade>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<ResultadoOperacao> RequisicaoAmizade(ConsultaAmigo itemAmigo)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Amigo/AjustarAmigo"));
            var json = JsonConvert.SerializeObject(itemAmigo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarRequisicaoAmizade(RequisicaoAmizade itemAmigo)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/RequisicaoAmizade/Post"));
            var json = JsonConvert.SerializeObject(itemAmigo, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<List<Usuario>> ListarUsuarios(CriterioBusca criterioBusca)
        {
            var ListaViagem = new List<Usuario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Usuario/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Usuario>>(resultado);
                ListaViagem = itemResultado.Lista;
            }

            return ListaViagem;
        }

        public async Task<List<Usuario>> ListarParticipantesViagem()
        {
            var ListaViagem = new List<Usuario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/CarregarParticipantes"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
                ListaViagem = itemResultado;
            }

            return ListaViagem;
        }

        public async Task<ResultadoOperacao> SalvarAmigo(ConsultaAmigo itemAmigo)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Amigo/Post"));
            var json = JsonConvert.SerializeObject(itemAmigo, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }


        public async Task<List<CotacaoMoeda>> ListarCotacaoMoeda()
        {
            var ListaAmigos = new List<CotacaoMoeda>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CotacaoMoeda/get?json={}"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<CotacaoMoeda>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<CotacaoMoeda> CarregarCotacaoMoeda(int? Identificador)
        {
            var itemCotacaoMoeda = new CotacaoMoeda();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CotacaoMoeda/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemCotacaoMoeda = JsonConvert.DeserializeObject<CotacaoMoeda>(resultado);

            }

            return itemCotacaoMoeda;
        }

        public async Task<ResultadoOperacao> ExcluirCotacaoMoeda(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CotacaoMoeda/",Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCotacaoMoeda(CotacaoMoeda itemCotacaoMoeda)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CotacaoMoeda/Post"));
            var json = JsonConvert.SerializeObject(itemCotacaoMoeda, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }



        public async Task<List<ListaCompra>> ListarListaCompra(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<ListaCompra>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<List<ListaCompra>> ListarPedidosCompraRecebido(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/CarregarPedidosRecebidos?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaAmigos = JsonConvert.DeserializeObject<List<ListaCompra>>(resultado);
            }

            return ListaAmigos;
        }

        public async Task<ListaCompra> CarregarListaCompra(int? Identificador)
        {
            var itemListaCompra = new ListaCompra();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemListaCompra = JsonConvert.DeserializeObject<ListaCompra>(resultado);

            }

            return itemListaCompra;
        }



        public async Task<ResultadoOperacao> ExcluirListaCompra(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarListaCompra(ListaCompra itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/Post"));
            var json = JsonConvert.SerializeObject(itemPedidoCompra, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<List<Cidade>> ListarCidadePai()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/ListarCidadePai"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeNaoAssociadasFilho()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/ListarCidadeNaoAssociadasFilho"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeNaoAssociadasPai(int id)
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/ListarCidadeNaoAssociadasPai/", id));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<ResultadoOperacao> ExcluirCidadeGrupo(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CidadeGrupo/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ManutencaoCidadeGrupo> CarregarCidadeGrupo(int? Identificador)
        {
            var itemListaCompra = new ManutencaoCidadeGrupo();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CidadeGrupo/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemListaCompra = JsonConvert.DeserializeObject<ManutencaoCidadeGrupo>(resultado);

            }

            return itemListaCompra;
        }

        public async Task<Cidade> CarregarCidade(int? Identificador)
        {
            var itemListaCompra = new Cidade();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemListaCompra = JsonConvert.DeserializeObject<Cidade>(resultado);

            }

            return itemListaCompra;
        }

        public async Task<ResultadoOperacao> SalvarCidadeGrupo(ManutencaoCidadeGrupo itemCidadeGrupo)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CidadeGrupo/Post"));
            var json = JsonConvert.SerializeObject(itemCidadeGrupo, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

    }
}
