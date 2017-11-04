using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CV.Mobile.Helpers;
using Plugin.Connectivity;

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

        public async Task<ResultadoOperacao> SalvarViagemSimples(Viagem itemViagem)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/SalvarSimples"));
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

        public async Task<ResultadoOperacao> SalvarUsuario(Usuario itemUsuario)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Usuario/Post"));
            var json = JsonConvert.SerializeObject(itemUsuario, new JsonSerializerSettings
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

        public async Task<Usuario> CarregarUsuario(int? Identificador)
        {
            var itemUsuario = new Usuario();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Usuario/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemUsuario = JsonConvert.DeserializeObject<Usuario>(resultado);

            }

            return itemUsuario;
        }


        public async Task<Boolean> VerificarOnLine()
        {
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Acesso/VerificaOnline"));

            // bool valido = await CrossConnectivity.Current.IsReachable(string.Concat("http://", uri.Host));

            // return valido;
            bool retorno = false;
            try
            {
                HttpResponseMessage response = null;
                client.Timeout = TimeSpan.FromSeconds(4);
                response = await client.GetAsync(uri);
                var resultado = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    retorno = JsonConvert.DeserializeObject<bool>(resultado);
                }
            }
            catch
            { }
            client.Timeout = TimeSpan.FromMinutes(1);

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

        public async Task<List<Amigo>> ListarAmigosUsuario()
        {
            var ListaAmigos = new List<Amigo>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Amigo/ListarAmigos"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaAmigos = JsonConvert.DeserializeObject<List<Amigo>>(resultado);
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

        public async Task<List<Usuario>> CarregarParticipantesAmigo()
        {
            var ListaViagem = new List<Usuario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Viagem/CarregarParticipantesAmigo"));
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


        public async Task<List<AporteDinheiro>> ListarAporteDinheiro(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<AporteDinheiro>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/AporteDinheiro/get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<AporteDinheiro>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<AporteDinheiro> CarregarAporteDinheiro(int? Identificador)
        {
            var itemAporteDinheiro = new AporteDinheiro();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/AporteDinheiro/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemAporteDinheiro = JsonConvert.DeserializeObject<AporteDinheiro>(resultado);

            }

            return itemAporteDinheiro;
        }

        public async Task<ResultadoOperacao> ExcluirAporteDinheiro(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/AporteDinheiro/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/AporteDinheiro/Post"));
            var json = JsonConvert.SerializeObject(itemAporteDinheiro, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> SalvarPosicao(Posicao itemPosicao)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Posicao/Post"));
            var json = JsonConvert.SerializeObject(itemPosicao, new JsonSerializerSettings
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

        public async Task<List<Sugestao>> ListarSugestao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Sugestao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Sugestao>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<List<Sugestao>> ListarSugestaoRecebida(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Sugestao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/listarConsulta?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaAmigos = JsonConvert.DeserializeObject<List<Sugestao>>(resultado);
            }

            return ListaAmigos;
        }

        public async Task<Sugestao> CarregarSugestao(int? Identificador)
        {
            var itemSugestao = new Sugestao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemSugestao = JsonConvert.DeserializeObject<Sugestao>(resultado);

            }

            return itemSugestao;
        }



        public async Task<ResultadoOperacao> ExcluirSugestao(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarSugestao(Sugestao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/Post"));
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

        public async Task<List<Cidade>> ListarCidadeSugestao()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarSugestao"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeFoto()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarFoto"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeAtracao()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarAtracao"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeRefeicao()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarRefeicao"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeHotel()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarHotel"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeViagemAerea()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarViagemAerea"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeLoja()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarLoja"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<Cidade>> ListarCidadeComentario()
        {
            var Lista = new List<Cidade>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Cidade/CarregarComentario"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Cidade>>(resultado);
            }

            return Lista;
        }

        public async Task<List<CalendarioPrevisto>> ListarCalendarioPrevisto(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<CalendarioPrevisto>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CalendarioPrevisto/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<CalendarioPrevisto>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

      

        public async Task<CalendarioPrevisto> CarregarCalendarioPrevisto(int? Identificador)
        {
            var itemCalendarioPrevisto = new CalendarioPrevisto();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CalendarioPrevisto/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemCalendarioPrevisto = JsonConvert.DeserializeObject<CalendarioPrevisto>(resultado);

            }

            return itemCalendarioPrevisto;
        }



        public async Task<ResultadoOperacao> ExcluirCalendarioPrevisto(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CalendarioPrevisto/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCalendarioPrevisto(CalendarioPrevisto itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/CalendarioPrevisto/Post"));
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

        public async Task<ResultadoOperacao> SalvarAgendarSugestao(AgendarSugestao itemAgenda)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sugestao/AgendarSugestao"));
            var json = JsonConvert.SerializeObject(itemAgenda, new JsonSerializerSettings
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


        public async Task<List<Atracao>> ListarAtracao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Atracao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Atracao/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Atracao>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }
        
        public async Task<Atracao> CarregarAtracao(int? Identificador)
        {
            var itemAtracao = new Atracao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Atracao/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemAtracao = JsonConvert.DeserializeObject<Atracao>(resultado);

            }

            return itemAtracao;
        }
        
        public async Task<ResultadoOperacao> ExcluirAtracao(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Atracao/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAtracao(Atracao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Atracao/Post"));
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


        public async Task<Atracao> VerificarAtracaoAberto()
        {
            var itemAtracao = new Atracao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Atracao/VerificarAtracaoAberto"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemAtracao = JsonConvert.DeserializeObject<Atracao>(resultado);

            }

            return itemAtracao;
        }


        public async Task<ResultadoOperacao> SalvarGastoAtracao(GastoAtracao item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/SalvarCustoAtracao"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<List<Gasto>> ListarGasto(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Gasto>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Gasto>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Gasto> CarregarGasto(int? Identificador)
        {
            var itemGasto = new Gasto();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemGasto = JsonConvert.DeserializeObject<Gasto>(resultado);

            }

            return itemGasto;
        }

        public async Task<ResultadoOperacao> ExcluirGasto(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGasto(Gasto itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/Post"));
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

        public async Task<List<Hotel>> ListarHotel(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Hotel>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Hotel>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Hotel> CarregarHotel(int? Identificador)
        {
            var itemHotel = new Hotel();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemHotel = JsonConvert.DeserializeObject<Hotel>(resultado);

            }

            return itemHotel;
        }

        public async Task<ResultadoOperacao> ExcluirHotel(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarHotel(Hotel itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/Post"));
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

        public async Task<ResultadoOperacao> SalvarGastoHotel(GastoHotel item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/SalvarCustoHotel"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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


        public async Task<ResultadoOperacao> SalvarHotelEvento(HotelEvento item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/SalvarHotelEventoVerificacao"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<HotelEvento> CarregarHotelEvento(int? Identificador)
        {
            var itemRefeicao = new HotelEvento();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Hotel/getHotelEvento/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<HotelEvento>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<List<Refeicao>> ListarRefeicao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Refeicao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Refeicao/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Refeicao>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Refeicao> CarregarRefeicao(int? Identificador)
        {
            var itemRefeicao = new Refeicao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Refeicao/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<Refeicao>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<ResultadoOperacao> ExcluirRefeicao(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Refeicao/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarRefeicao(Refeicao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Refeicao/Post"));
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

        public async Task<ResultadoOperacao> SalvarGastoRefeicao(GastoRefeicao item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/SalvarCustoRefeicao"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> SalvarAluguelGasto(AluguelGasto item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/SalvarCustoCarro"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<List<Comentario>> ListarComentario(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Comentario>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Comentario/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Comentario>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Comentario> CarregarComentario(int? Identificador)
        {
            var itemComentario = new Comentario();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Comentario/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemComentario = JsonConvert.DeserializeObject<Comentario>(resultado);

            }

            return itemComentario;
        }

        public async Task<ResultadoOperacao> ExcluirComentario(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Comentario/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarComentario(Comentario itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Comentario/Post"));
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


        public async Task<List<Loja>> ListarLoja(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Loja>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Loja>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Loja> CarregarLoja(int? Identificador)
        {
            var itemLoja = new Loja();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemLoja = JsonConvert.DeserializeObject<Loja>(resultado);

            }

            return itemLoja;
        }

        public async Task<ResultadoOperacao> ExcluirLoja(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarLoja(Loja itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/Post"));
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

        public async Task<ResultadoOperacao> SalvarGastoCompra(GastoCompra item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/saveCompra"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> ExcluirGastoCompra(GastoCompra item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/excluirCompra"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> SalvarItemCompra(ItemCompra item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/SalvarItemCompra"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<List<ListaCompra>> CarregarListaPedidos(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ListaCompra/CarregarListaPedidos?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> ExcluirReabastecimento(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Reabastecimento/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarReabastecimento(Reabastecimento itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Reabastecimento/Post"));
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

        public async Task<Reabastecimento> CarregarReabastecimento(int? Identificador)
        {
            var itemReabastecimento = new Reabastecimento();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Reabastecimento/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemReabastecimento = JsonConvert.DeserializeObject<Reabastecimento>(resultado);

            }

            return itemReabastecimento;
        }


        public async Task<ResultadoOperacao> SalvarCarroDeslocamento(CarroDeslocamento itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/SalvarCarroDeslocamento"));
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

        public async Task<CarroDeslocamento> CarregarCarroDeslocamento(int? Identificador)
        {
            var itemCarroDeslocamento = new CarroDeslocamento();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/CarregarCarroDeslocamento/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemCarroDeslocamento = JsonConvert.DeserializeObject<CarroDeslocamento>(resultado);

            }

            return itemCarroDeslocamento;
        }



        public async Task<List<Carro>> ListarCarro(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Carro>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Carro>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<Carro> CarregarCarro(int? Identificador)
        {
            var itemCarro = new Carro();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemCarro = JsonConvert.DeserializeObject<Carro>(resultado);

            }

            return itemCarro;
        }

        public async Task<ResultadoOperacao> ExcluirCarro(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCarro(Carro itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Carro/Post"));
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

        public async Task<List<ViagemAerea>> ListarViagemAerea(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ViagemAerea>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ViagemAerea/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }))); var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<ViagemAerea>>(resultado);
                ListaAmigos = itemResultado.Lista;
            }

            return ListaAmigos;
        }

        public async Task<ViagemAerea> CarregarViagemAerea(int? Identificador)
        {
            var itemViagemAerea = new ViagemAerea();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ViagemAerea/get/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemViagemAerea = JsonConvert.DeserializeObject<ViagemAerea>(resultado);

            }

            return itemViagemAerea;
        }

        public async Task<ItemCompra> CarregarItemCompra(int? Identificador)
        {
            var itemViagemAerea = new ItemCompra();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Loja/CarregarItemCompra/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemViagemAerea = JsonConvert.DeserializeObject<ItemCompra>(resultado);

            }

            return itemViagemAerea;
        }

        public async Task<ResultadoOperacao> ExcluirViagemAerea(int? Identificador)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ViagemAerea/", Identificador));

            HttpResponseMessage response = null;
            response = await client.DeleteAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<ResultadoOperacao>(resultado);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarViagemAerea(ViagemAerea itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/ViagemAerea/Post"));
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

        public async Task<ResultadoOperacao> SalvarGastoViagemAerea(GastoViagemAerea item)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/SalvarCustoViagemAerea"));
            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> SubirImagem(UploadFoto itemUpload)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/SubirImagemDireto"));
            var json = JsonConvert.SerializeObject(itemUpload, new JsonSerializerSettings
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

        public async Task<ResultadoOperacao> SubirVideo(UploadFoto itemUpload)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/SubirVideo"));
            var json = JsonConvert.SerializeObject(itemUpload, new JsonSerializerSettings
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

        public async Task<ClasseSincronizacao> RetornarAtualizacoes(CriterioBusca criterioBusca)
        {
            ClasseSincronizacao itemRetorno = new ClasseSincronizacao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sincronizacao/RetornarAtualizacoes?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRetorno = JsonConvert.DeserializeObject<ClasseSincronizacao>(resultado);
            }

            return itemRetorno;
        }

        public async Task<List<DeParaIdentificador>> SincronizarDados(ClasseSincronizacao itemSincronizar)
        {
            List<DeParaIdentificador> itemResultado = new List<DeParaIdentificador>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Sincronizacao/SincronizarDados"));
            var json = JsonConvert.SerializeObject(itemSincronizar, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.Timeout = TimeSpan.FromMinutes(5);
            HttpResponseMessage response = null;
            response = await client.PostAsync(uri, content);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemResultado = JsonConvert.DeserializeObject<List<DeParaIdentificador>>(resultado);
            }


            return itemResultado;
        }

        public async Task<GastoAtracao> CarregarGastoAtracao(int? Identificador)
        {
            var itemRefeicao = new GastoAtracao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getGastoAtracao/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<GastoAtracao>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<GastoHotel> CarregarGastoHotel(int? Identificador)
        {
            var itemRefeicao = new GastoHotel();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getGastoHotel/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<GastoHotel>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<GastoCompra> CarregarGastoCompra(int? Identificador)
        {
            var itemRefeicao = new GastoCompra();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getGastoCompra/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<GastoCompra>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<GastoRefeicao> CarregarGastoRefeicao(int? Identificador)
        {
            var itemRefeicao = new GastoRefeicao();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getGastoRefeicao/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<GastoRefeicao>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<GastoViagemAerea> CarregarGastoViagemAerea(int? Identificador)
        {
            var itemRefeicao = new GastoViagemAerea();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getGastoViagemAerea/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<GastoViagemAerea>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<AluguelGasto> CarregarAluguelGasto(int? Identificador)
        {
            var itemRefeicao = new AluguelGasto();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Gasto/getAluguelGasto/", Identificador.GetValueOrDefault(0)));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemRefeicao = JsonConvert.DeserializeObject<AluguelGasto>(resultado);

            }

            return itemRefeicao;
        }

        public async Task<List<ExtratoMoeda>> ListarExtratoMoeda(CriterioBusca criterioBusca)
        {
            var Lista = new List<ExtratoMoeda>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarExtratoMoeda?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<ExtratoMoeda>>(resultado);
                
            }

            return Lista;
        }

        public async Task<List<AjusteGastoDividido>> ListarAjusteGastos(CriterioBusca criterioBusca)
        {
            var Lista = new List<AjusteGastoDividido>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarGastosAcerto?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<AjusteGastoDividido>>(resultado);

            }

            return Lista;
        }

        public async Task<List<RelatorioGastos>> ListarRelatorioGastos(CriterioBusca criterioBusca)
        {
            var Lista = new List<RelatorioGastos>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarRelatorioGastos?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<RelatorioGastos>>(resultado);

            }

            return Lista;
        }
        public async Task<List<CalendarioRealizado>> ConsultarCalendarioRealizado(CriterioBusca criterioBusca)
        {
            var Lista = new List<CalendarioRealizado>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarCalendarioRealizado?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<CalendarioRealizado>>(resultado);

            }

            return Lista;
        }

        public async Task<ResumoViagem> CarregarResumo(CriterioBusca criterioBusca)
        {
            var item = new ResumoViagem();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/CarregarResumo?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                item = JsonConvert.DeserializeObject<ResumoViagem>(resultado);

            }

            return item;
        }

        public async Task<List<ConsultaRankings>> ListarRankings(CriterioBusca criterioBusca)
        {
            var Lista = new List<ConsultaRankings>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarRankings?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<ConsultaRankings>>(resultado);

            }

            return Lista;
        }
        public async Task<List<UsuarioConsulta>> ListarAvaliacoesRankings(CriterioBusca criterioBusca)
        {
            var Lista = new List<UsuarioConsulta>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarAvaliacoesRankings?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<UsuarioConsulta>>(resultado);

            }

            return Lista;
        }

        public async Task<List<LocaisVisitados>> ListarLocaisVisitados(CriterioBusca criterioBusca)
        {
            var Lista = new List<LocaisVisitados>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarLocaisVisitados?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<LocaisVisitados>>(resultado);

            }

            return Lista;
        }

        public async Task<LocaisVisitados> ConsultarDetalheAtracao(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarDetalheAtracao?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                item = JsonConvert.DeserializeObject<LocaisVisitados>(resultado);

            }

            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheHotel(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarDetalheHotel?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                item = JsonConvert.DeserializeObject<LocaisVisitados>(resultado);

            }

            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheRestaurante(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarDetalheRestaurante?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                item = JsonConvert.DeserializeObject<LocaisVisitados>(resultado);

            }

            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheLoja(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarDetalheLoja?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                item = JsonConvert.DeserializeObject<LocaisVisitados>(resultado);

            }

            return item;
        }
        public async Task<List<Foto>> ListarFoto(CriterioBusca criterioBusca)
        {
            var ListaFoto = new List<Foto>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/Get?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var itemResultado = JsonConvert.DeserializeObject<ResultadoConsultaTipo<Foto>>(resultado);
                ListaFoto = itemResultado.Lista;
            }

            return ListaFoto;
        }

        public async Task<List<Atracao>> CarregarFotoAtracao()
        {
            var ListaFoto = new List<Atracao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/CarregarFoto"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaFoto = JsonConvert.DeserializeObject<List<Atracao>>(resultado);

            }

            return ListaFoto;
        }

        public async Task<List<Hotel>> CarregarFotoHotel()
        {
            var ListaFoto = new List<Hotel>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/CarregarFoto"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaFoto = JsonConvert.DeserializeObject<List<Hotel>>(resultado);

            }

            return ListaFoto;
        }

        public async Task<List<Refeicao>> CarregarFotoRefeicao()
        {
            var ListaFoto = new List<Refeicao>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/CarregarFoto"));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ListaFoto = JsonConvert.DeserializeObject<List<Refeicao>>(resultado);

            }

            return ListaFoto;
        }

        public async Task<List<Timeline>> ConsultarTimeline(CriterioBusca criterioBusca)
        {
            var Lista = new List<Timeline>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ConsultarTimeline?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<Timeline>>(resultado);

            }

            return Lista;
        }

        public async Task<Foto> CarregarFoto(int? Identificador)
        {
            Foto itemFoto = new Foto();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Foto/Get/", Identificador));

            HttpResponseMessage response = null;
            response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                itemFoto = JsonConvert.DeserializeObject<Foto>(resultado);
            }


            return itemFoto;
        }

        public async Task<List<PontoMapa>> ListarPontosViagem(CriterioBusca criterioBusca)
        {
            var Lista = new List<PontoMapa>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarPontosViagem?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<PontoMapa>>(resultado);

            }

            return Lista;
        }

        public async Task<List<LinhaMapa>> ListarLinhasViagem(CriterioBusca criterioBusca)
        {
            var Lista = new List<LinhaMapa>();
            var uri = new Uri(String.Concat(Settings.BaseWebApi, "Api/Consulta/ListarLinhasViagem?json=", JsonConvert.SerializeObject(criterioBusca, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            })));
            var response = await client.GetAsync(uri);
            var resultado = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Lista = JsonConvert.DeserializeObject<List<LinhaMapa>>(resultado);

            }

            return Lista;
        }

    }
}
