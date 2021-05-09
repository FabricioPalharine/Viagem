using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Services.Dialog;
using CV.Mobile.Services.RequestProvider;
using CV.Mobile.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.Services.Api
{
    public class ApiService: IApiService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly IDialogService _messagingCenter;
        private JsonSerializerSettings jsonSerializerSettings;
        public ApiService(IRequestProvider requestProvider, IDialogService messagingCenter)
        {
            _requestProvider = requestProvider;
            _messagingCenter = messagingCenter;
            jsonSerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }

        public async Task<UsuarioLogado> CarregarDadosAplicativo(UsuarioLogado DadosLogin)
        {
            UsuarioLogado usuarioLogado = new UsuarioLogado();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"api/Acesso/CarregarDadosAplicativo");
           
                usuarioLogado = await _requestProvider.PostAsync<UsuarioLogado>(uri, DadosLogin, GlobalSetting.Instance.AuthToken);
                GlobalSetting.Instance.AuthToken = usuarioLogado.AuthenticationToken;
                await SecureStorage.SetAsync("AuthenticationToken", usuarioLogado.AuthenticationToken);
           
            return usuarioLogado;
        }

        public async Task<UsuarioLogado> LogarUsuario(DadosGoogleToken DadosLogin)
        {
            UsuarioLogado usuarioLogado = new UsuarioLogado();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"api/Acesso/LoginAplicativo");
            
                usuarioLogado = await _requestProvider.PostAsync<DadosGoogleToken, UsuarioLogado>(uri, DadosLogin);
                GlobalSetting.Instance.AuthToken = usuarioLogado.AuthenticationToken;
                await SecureStorage.SetAsync("AuthenticationToken", usuarioLogado.AuthenticationToken);
           
            return usuarioLogado;
        }

        public async Task<ResultadoOperacao> SalvarUsuario(Usuario itemUsuario)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"api/Usuario/Post");
                var resultado = await _requestProvider.PostAsync<Usuario, ResultadoOperacao> (uri, itemUsuario, GlobalSetting.Instance.AuthToken);
                return resultado;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        private async Task ExibirMensagemErro(Exception ex)
        {
            Debug.WriteLine(ex);
            await _messagingCenter.ShowAlertAsync("Ocorreu um erro inexperado na comunicação com o servidor. Tente Novamente mais tarde", "Erro Comunicação", "OK");
        }

        public async Task<List<Viagem>> ListarViagens(CriterioBusca criterioBusca)
        {
            var ListaViagem = new List<Viagem>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaViagem = (await _requestProvider.GetAsync<ResultadoConsultaTipo<Viagem>>(uri, GlobalSetting.Instance.AuthToken)).Lista;
                
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaViagem;
        }

        public async Task<List<Usuario>> ListarAmigos()
        {
            var ListaAmigos = new List<Usuario>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Usuario/listaAmigos");
                ListaAmigos = await _requestProvider.GetAsync<List<Usuario>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<List<Usuario>> ListarAmigosComigo()
        {
            var ListaAmigos = new List<Usuario>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Usuario/listaAmigosComigo");
            try
            {
                ListaAmigos = (await _requestProvider.GetAsync<ResultadoConsultaTipo<Usuario>>(uri, GlobalSetting.Instance.AuthToken)).Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<UsuarioLogado> SelecionarViagem(int? IdentificadorViagem)
        {
            PocoLogin itemLogin = new PocoLogin();
            itemLogin.IdentificadorViagem = IdentificadorViagem;
            UsuarioLogado itemUsuario = new UsuarioLogado();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Acesso/SelecionarViagem");
            try
            {
                itemUsuario = await _requestProvider.PostAsync<PocoLogin, UsuarioLogado>(uri, itemLogin, GlobalSetting.Instance.AuthToken);
                GlobalSetting.Instance.AuthToken = itemUsuario.AuthenticationToken;
                await SecureStorage.SetAsync("AuthenticationToken", itemUsuario.AuthenticationToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemUsuario;
        }

        public async Task<UsuarioLogado> LoginGoogle(PocoLogin itemLogin)
        {
            UsuarioLogado itemUsuario = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Acesso/LoginGoogle");
            try
            {
                itemUsuario = await _requestProvider.PostAsync<PocoLogin, UsuarioLogado>(uri, itemLogin);
                GlobalSetting.Instance.AuthToken = itemUsuario.AuthenticationToken;
                await SecureStorage.SetAsync("AuthenticationToken", itemUsuario.AuthenticationToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemUsuario;
        }

        public async Task<Viagem> CarregarViagem(int? IdentificadorViagem)
        {
            Viagem itemViagem = new Viagem();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/Get/{IdentificadorViagem}");
            try
            {
                itemViagem = await _requestProvider.GetAsync<Viagem>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemViagem;
        }
        

        public async Task<ResultadoOperacao> SalvarViagem(Viagem itemViagem)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Viagem, ResultadoOperacao>(uri, itemViagem, GlobalSetting.Instance.AuthToken);
                
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;

        }

        public async Task<ResultadoOperacao> SalvarViagemSimples(Viagem itemViagem)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/SalvarSimples");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Viagem, ResultadoOperacao>(uri, itemViagem, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        

        public async Task<Usuario> CarregarUsuario(int? Identificador)
        {
            var itemUsuario = new Usuario();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Usuario/get/{Identificador.GetValueOrDefault(0)}");
                itemUsuario = await _requestProvider.GetAsync<Usuario>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemUsuario;
        }


        public async Task<Boolean> VerificarOnLine()
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Acesso/VerificaOnline");

           
            bool retorno = false;
            try
            {
                retorno = await _requestProvider.GetAsync<bool>(uri);
            }
            catch
            { 
            }
           

            return retorno;
        }

        public async Task<List<ConsultaAmigo>> ListarAmigosConsulta()
        {
            var ListaAmigos = new List<ConsultaAmigo>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, "Api/Amigo/get?json={}");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<ConsultaAmigo>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<List<Amigo>> ListarAmigosUsuario()
        {
            var ListaAmigos = new List<Amigo>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Amigo/ListarAmigos");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Amigo>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<List<RequisicaoAmizade>> ListarRequisicaoAmizade()
        {
            var ListaAmigos = new List<RequisicaoAmizade>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, "Api/RequisicaoAmizade/get?json={}");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<RequisicaoAmizade>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<ResultadoOperacao> RequisicaoAmizade(ConsultaAmigo itemAmigo)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Amigo/AjustarAmigo");
                var json = JsonConvert.SerializeObject(itemAmigo);
                itemResultado = await _requestProvider.PostAsync<ConsultaAmigo, ResultadoOperacao>(uri, itemAmigo, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarRequisicaoAmizade(RequisicaoAmizade itemAmigo)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/RequisicaoAmizade/Post");
                itemResultado = await _requestProvider.PostAsync<RequisicaoAmizade, ResultadoOperacao>(uri, itemAmigo, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<List<Usuario>> ListarUsuarios(CriterioBusca criterioBusca)
        {
            var ListaViagem = new List<Usuario>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Usuario/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Usuario>>(uri, GlobalSetting.Instance.AuthToken);
                ListaViagem = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaViagem;
        }

        public async Task<List<Usuario>> ListarParticipantesViagem()
        {
            var ListaViagem = new List<Usuario>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/CarregarParticipantes");
                ListaViagem = await _requestProvider.GetAsync<List<Usuario>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaViagem;
        }

        public async Task<List<Usuario>> CarregarParticipantesAmigo()
        {
            var ListaViagem = new List<Usuario>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Viagem/CarregarParticipantesAmigo");
                ListaViagem = await _requestProvider.GetAsync<List<Usuario>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaViagem;
        }

        public async Task<ResultadoOperacao> SalvarAmigo(ConsultaAmigo itemAmigo)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Amigo/Post");
                itemResultado = await _requestProvider.PostAsync<ConsultaAmigo, ResultadoOperacao>(uri, itemAmigo, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }


        public async Task<List<CotacaoMoeda>> ListarCotacaoMoeda(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<CotacaoMoeda>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CotacaoMoeda/get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
            try
            {
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<CotacaoMoeda>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<CotacaoMoeda> CarregarCotacaoMoeda(int? Identificador)
        {
            var itemCotacaoMoeda = new CotacaoMoeda();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CotacaoMoeda/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemCotacaoMoeda = await _requestProvider.GetAsync<CotacaoMoeda>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemCotacaoMoeda;
        }

        public async Task<ResultadoOperacao> ExcluirCotacaoMoeda(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CotacaoMoeda/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCotacaoMoeda(CotacaoMoeda itemCotacaoMoeda)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CotacaoMoeda/Post");
                itemResultado = await _requestProvider.PostAsync<CotacaoMoeda, ResultadoOperacao>(uri, itemCotacaoMoeda, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }



        public async Task<List<ListaCompra>> ListarListaCompra(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaAmigos = await _requestProvider.GetAsync<List<ListaCompra>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<List<ListaCompra>> ListarPedidosCompraRecebido(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/CarregarPedidosRecebidos?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaAmigos = await _requestProvider.GetAsync<List<ListaCompra>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<ListaCompra> CarregarListaCompra(int? Identificador)
        {
            var itemListaCompra = new ListaCompra();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/get/{ Identificador.GetValueOrDefault(0)}");
            try
            {
                itemListaCompra = await _requestProvider.GetAsync<ListaCompra>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemListaCompra;
        }



        public async Task<ResultadoOperacao> ExcluirListaCompra(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarListaCompra(ListaCompra itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;

            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<ListaCompra, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }



            return itemResultado;
        }

       
        public async Task<List<AporteDinheiro>> ListarAporteDinheiro(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<AporteDinheiro>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/AporteDinheiro/get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
            try
            {
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<AporteDinheiro>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<AporteDinheiro> CarregarAporteDinheiro(int? Identificador)
        {
            var itemAporteDinheiro = new AporteDinheiro();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/AporteDinheiro/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemAporteDinheiro = await _requestProvider.GetAsync<AporteDinheiro>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemAporteDinheiro;
        }

        public async Task<ResultadoOperacao> ExcluirAporteDinheiro(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/AporteDinheiro/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/AporteDinheiro/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<AporteDinheiro, ResultadoOperacao>(uri, itemAporteDinheiro, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarPosicao(Posicao itemPosicao)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Posicao/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Posicao, ResultadoOperacao>(uri, itemPosicao, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<List<Sugestao>> ListarSugestao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Sugestao>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaAmigos = await _requestProvider.GetAsync<List<Sugestao>>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<List<Sugestao>> ListarSugestaoRecebida(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Sugestao>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/listarConsulta?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaAmigos = await _requestProvider.GetAsync<List<Sugestao>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<Sugestao> CarregarSugestao(int? Identificador)
        {
            var itemSugestao = new Sugestao();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemSugestao = await _requestProvider.GetAsync<Sugestao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemSugestao;
        }



        public async Task<ResultadoOperacao> ExcluirSugestao(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarSugestao(Sugestao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Sugestao, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }


        public async Task<List<CalendarioPrevisto>> ListarCalendarioPrevisto(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<CalendarioPrevisto>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CalendarioPrevisto/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
            try
            {
                ListaAmigos = await _requestProvider.GetAsync<List<CalendarioPrevisto>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }



        public async Task<CalendarioPrevisto> CarregarCalendarioPrevisto(int? Identificador)
        {
            var itemCalendarioPrevisto = new CalendarioPrevisto();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CalendarioPrevisto/get/{Identificador.GetValueOrDefault(0)}");
                itemCalendarioPrevisto = await _requestProvider.GetAsync<CalendarioPrevisto>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemCalendarioPrevisto;
        }



        public async Task<ResultadoOperacao> ExcluirCalendarioPrevisto(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CalendarioPrevisto/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCalendarioPrevisto(CalendarioPrevisto itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/CalendarioPrevisto/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<CalendarioPrevisto, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAgendarSugestao(AgendarSugestao itemAgenda)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sugestao/AgendarSugestao");
                itemResultado = await _requestProvider.PostAsync<AgendarSugestao, ResultadoOperacao>(uri, itemAgenda, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }


        public async Task<List<Atracao>> ListarAtracao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Atracao>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Atracao/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Atracao>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<Atracao> CarregarAtracao(int? Identificador)
        {
            var itemAtracao = new Atracao();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Atracao/get/{Identificador.GetValueOrDefault(0)}");
                itemAtracao = await _requestProvider.GetAsync<Atracao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemAtracao;
        }

        public async Task<ResultadoOperacao> ExcluirAtracao(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Atracao/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAtracao(Atracao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Atracao/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Atracao, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }


        public async Task<Atracao> VerificarAtracaoAberto()
        {
            var itemAtracao = new Atracao();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Atracao/VerificarAtracaoAberto");
                itemAtracao = await _requestProvider.GetAsync<Atracao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemAtracao;
        }


        public async Task<ResultadoOperacao> SalvarGastoAtracao(GastoAtracao item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/SalvarCustoAtracao");
            try
            {
                itemResultado = await _requestProvider.PostAsync<GastoAtracao, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<List<Gasto>> ListarGasto(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Gasto>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/Get?json={JsonConvert.SerializeObject(criterioBusca,jsonSerializerSettings)}");
            try
            {
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Gasto>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<Gasto> CarregarGasto(int? Identificador)
        {
            var itemGasto = new Gasto();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemGasto = await _requestProvider.GetAsync<Gasto>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemGasto;
        }

        public async Task<ResultadoOperacao> ExcluirGasto(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGasto(Gasto itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Gasto, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);


            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<List<Hotel>> ListarHotel(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Hotel>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Hotel>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<Hotel> CarregarHotel(int? Identificador)
        {
            var itemHotel = new Hotel();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/get/{Identificador.GetValueOrDefault(0)}");
                itemHotel = await _requestProvider.GetAsync<Hotel>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemHotel;
        }

        public async Task<ResultadoOperacao> ExcluirHotel(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarHotel(Hotel itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Hotel, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGastoHotel(GastoHotel item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/SalvarCustoHotel");
            try
            {
                itemResultado = await _requestProvider.PostAsync<GastoHotel, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }


        public async Task<ResultadoOperacao> SalvarHotelEvento(HotelEvento item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/SalvarHotelEventoVerificacao");
            try
            {
                itemResultado = await _requestProvider.PostAsync<HotelEvento, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<HotelEvento> CarregarHotelEvento(int? Identificador)
        {
            var itemRefeicao = new HotelEvento();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Hotel/getHotelEvento/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<HotelEvento>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<List<Refeicao>> ListarRefeicao(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Refeicao>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Refeicao/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
            try
            {
                ListaAmigos = (await _requestProvider.GetAsync<ResultadoConsultaTipo<Refeicao>>(uri, GlobalSetting.Instance.AuthToken)).Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<Refeicao> CarregarRefeicao(int? Identificador)
        {
            var itemRefeicao = new Refeicao();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Refeicao/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<Refeicao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<ResultadoOperacao> ExcluirRefeicao(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Refeicao/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarRefeicao(Refeicao itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Refeicao/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Refeicao, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGastoRefeicao(GastoRefeicao item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/SalvarCustoRefeicao");
            try
            {
                itemResultado = await _requestProvider.PostAsync<GastoRefeicao, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarAluguelGasto(AluguelGasto item)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/SalvarCustoCarro");
                itemResultado = await _requestProvider.PostAsync<AluguelGasto, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<List<Comentario>> ListarComentario(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Comentario>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Comentario/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
            
            try
            {
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<Comentario>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<Comentario> CarregarComentario(int? Identificador)
        {
            var itemComentario = new Comentario();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Comentario/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemComentario = await _requestProvider.GetAsync<Comentario>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemComentario;
        }

        public async Task<ResultadoOperacao> ExcluirComentario(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Comentario/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarComentario(Comentario itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Comentario/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Comentario, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }


        public async Task<List<Loja>> ListarLoja(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Loja>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
          
            try
            {
                ListaAmigos = await _requestProvider.GetAsync<List<Loja>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<Loja> CarregarLoja(int? Identificador)
        {
            var itemLoja = new Loja();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemLoja = await _requestProvider.GetAsync<Loja>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemLoja;
        }

        public async Task<ResultadoOperacao> ExcluirLoja(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarLoja(Loja itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Loja, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGastoCompra(GastoCompra item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/saveCompra");
            try
            {
                itemResultado = await _requestProvider.PostAsync<GastoCompra, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> ExcluirGastoCompra(GastoCompra item)
        {
            ResultadoOperacao itemResultado = null;
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/excluirCompra");
                itemResultado = await _requestProvider.PostAsync<GastoCompra, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarItemCompra(ItemCompra item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/SalvarItemCompra");
            try
            {
                itemResultado = await _requestProvider.PostAsync<ItemCompra, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<List<ListaCompra>> CarregarListaPedidos(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ListaCompra>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ListaCompra/CarregarListaPedidos?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
          
            try
            {
                ListaAmigos = await _requestProvider.GetAsync<List<ListaCompra>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<ResultadoOperacao> ExcluirReabastecimento(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Reabastecimento/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarReabastecimento(Reabastecimento itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Reabastecimento/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Reabastecimento, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<Reabastecimento> CarregarReabastecimento(int? Identificador)
        {
            var itemReabastecimento = new Reabastecimento();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Reabastecimento/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemReabastecimento = await _requestProvider.GetAsync<Reabastecimento>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemReabastecimento;
        }


        public async Task<ResultadoOperacao> SalvarCarroDeslocamento(CarroDeslocamento itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/SalvarCarroDeslocamento");
            try
            {
                itemResultado = await _requestProvider.PostAsync<CarroDeslocamento, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<CarroDeslocamento> CarregarCarroDeslocamento(int? Identificador)
        {
            var itemCarroDeslocamento = new CarroDeslocamento();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/CarregarCarroDeslocamento/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemCarroDeslocamento = await _requestProvider.GetAsync<CarroDeslocamento>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemCarroDeslocamento;
        }



        public async Task<List<Carro>> ListarCarro(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<Carro>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
           
            try
            {
                ListaAmigos = await _requestProvider.GetAsync<List<Carro>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaAmigos;
        }

        public async Task<Carro> CarregarCarro(int? Identificador)
        {
            var itemCarro = new Carro();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/get/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemCarro = await _requestProvider.GetAsync<Carro>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemCarro;
        }

        public async Task<ResultadoOperacao> ExcluirCarro(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarCarro(Carro itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Carro/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<Carro, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<List<ViagemAerea>> ListarViagemAerea(CriterioBusca criterioBusca)
        {
            var ListaAmigos = new List<ViagemAerea>();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ViagemAerea/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
           
            try
            {
                var resultado = await _requestProvider.GetAsync<ResultadoConsultaTipo<ViagemAerea>>(uri, GlobalSetting.Instance.AuthToken);
                ListaAmigos = resultado.Lista;
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return ListaAmigos;
        }

        public async Task<ViagemAerea> CarregarViagemAerea(int? Identificador)
        {
            var itemViagemAerea = new ViagemAerea();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ViagemAerea/get/{Identificador.GetValueOrDefault(0)}");
                itemViagemAerea = await _requestProvider.GetAsync<ViagemAerea>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemViagemAerea;
        }

        public async Task<ItemCompra> CarregarItemCompra(int? Identificador)
        {
            var itemViagemAerea = new ItemCompra();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Loja/CarregarItemCompra/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemViagemAerea = await _requestProvider.GetAsync<ItemCompra>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemViagemAerea;
        }

        public async Task<ResultadoOperacao> ExcluirViagemAerea(int? Identificador)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ViagemAerea/{Identificador}");
            try
            {
                itemResultado = await _requestProvider.DeleteAsync<ResultadoOperacao>(uri, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarViagemAerea(ViagemAerea itemPedidoCompra)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/ViagemAerea/Post");
            try
            {
                itemResultado = await _requestProvider.PostAsync<ViagemAerea, ResultadoOperacao>(uri, itemPedidoCompra, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SalvarGastoViagemAerea(GastoViagemAerea item)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/SalvarCustoViagemAerea");
            try
            {
                itemResultado = await _requestProvider.PostAsync<GastoViagemAerea, ResultadoOperacao>(uri, item, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<ResultadoOperacao> SubirImagem(UploadFoto itemUpload)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/SubirImagemDireto");
            try
            {
                itemResultado = await _requestProvider.PostAsync<UploadFoto, ResultadoOperacao>(uri, itemUpload, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemResultado;
        }

        public async Task<ResultadoOperacao> SubirVideo(UploadFoto itemUpload)
        {
            ResultadoOperacao itemResultado = null;
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/SubirVideo");
            try
            {
                itemResultado = await _requestProvider.PostAsync<UploadFoto, ResultadoOperacao>(uri, itemUpload, GlobalSetting.Instance.AuthToken);
            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemResultado;
        }

        public async Task<ClasseSincronizacao> RetornarAtualizacoes(CriterioBusca criterioBusca)
        {
            ClasseSincronizacao itemRetorno = new ClasseSincronizacao();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sincronizacao/RetornarAtualizacoes?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
  
            try
            {
                itemRetorno = await _requestProvider.GetAsync<ClasseSincronizacao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemRetorno;
        }

        public async Task<List<DeParaIdentificador>> SincronizarDados(ClasseSincronizacao itemSincronizar)
        {
            List<DeParaIdentificador> itemResultado = new List<DeParaIdentificador>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Sincronizacao/SincronizarDados");
                itemResultado = await _requestProvider.PostAsync<ClasseSincronizacao, List<DeParaIdentificador>>(uri, itemSincronizar, GlobalSetting.Instance.AuthToken,300000);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }


            return itemResultado;
        }

        public async Task<GastoAtracao> CarregarGastoAtracao(int? Identificador)
        {
            var itemRefeicao = new GastoAtracao();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getGastoAtracao/{ Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<GastoAtracao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<GastoHotel> CarregarGastoHotel(int? Identificador)
        {
            var itemRefeicao = new GastoHotel();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getGastoHotel/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<GastoHotel>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<GastoCompra> CarregarGastoCompra(int? Identificador)
        {
            var itemRefeicao = new GastoCompra();
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getGastoCompra/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<GastoCompra>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<GastoRefeicao> CarregarGastoRefeicao(int? Identificador)
        {
            var itemRefeicao = new GastoRefeicao();

            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getGastoRefeicao/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<GastoRefeicao>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemRefeicao;
        }

        public async Task<GastoViagemAerea> CarregarGastoViagemAerea(int? Identificador)
        {
            var itemRefeicao = new GastoViagemAerea();

            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getGastoViagemAerea/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<GastoViagemAerea>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemRefeicao;
        }

        public async Task<AluguelGasto> CarregarAluguelGasto(int? Identificador)
        {
            var itemRefeicao = new AluguelGasto();

            var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Gasto/getAluguelGasto/{Identificador.GetValueOrDefault(0)}");
            try
            {
                itemRefeicao = await _requestProvider.GetAsync<AluguelGasto>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return itemRefeicao;
        }

        public async Task<List<ExtratoMoeda>> ListarExtratoMoeda(CriterioBusca criterioBusca)
        {
            var Lista = new List<ExtratoMoeda>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarExtratoMoeda?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<ExtratoMoeda>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<List<AjusteGastoDividido>> ListarAjusteGastos(CriterioBusca criterioBusca)
        {
            var Lista = new List<AjusteGastoDividido>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarGastosAcerto?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<AjusteGastoDividido>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<List<RelatorioGastos>> ListarRelatorioGastos(CriterioBusca criterioBusca)
        {
            var Lista = new List<RelatorioGastos>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarRelatorioGastos?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<RelatorioGastos>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }
        public async Task<List<CalendarioRealizado>> ConsultarCalendarioRealizado(CriterioBusca criterioBusca)
        {
            var Lista = new List<CalendarioRealizado>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarCalendarioRealizado?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<CalendarioRealizado>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<ResumoViagem> CarregarResumo(CriterioBusca criterioBusca)
        {
            var item = new ResumoViagem();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/CarregarResumo?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                item = await _requestProvider.GetAsync<ResumoViagem>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return item;
        }

        
        public async Task<List<UsuarioConsulta>> ListarAvaliacoesRankings(CriterioBusca criterioBusca)
        {
            var Lista = new List<UsuarioConsulta>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarAvaliacoesRankings?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<UsuarioConsulta>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<List<LocaisVisitados>> ListarLocaisVisitados(CriterioBusca criterioBusca)
        {
            var Lista = new List<LocaisVisitados>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarLocaisVisitados?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<LocaisVisitados>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<LocaisVisitados> ConsultarDetalheAtracao(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarDetalheAtracao?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                item = await _requestProvider.GetAsync<LocaisVisitados>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheHotel(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarDetalheHotel?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                item = await _requestProvider.GetAsync<LocaisVisitados>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheRestaurante(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarDetalheRestaurante?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                item = await _requestProvider.GetAsync<LocaisVisitados>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return item;
        }

        public async Task<LocaisVisitados> ConsultarDetalheLoja(CriterioBusca criterioBusca)
        {
            var item = new LocaisVisitados();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarDetalheLoja?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                item = await _requestProvider.GetAsync<LocaisVisitados>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return item;
        }
        public async Task<List<Foto>> ListarFoto(CriterioBusca criterioBusca)
        {
            var ListaFoto = new List<Foto>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/Get?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                ListaFoto = (await _requestProvider.GetAsync<ResultadoConsultaTipo<Foto>>(uri, GlobalSetting.Instance.AuthToken)).Lista;

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaFoto;
        }

        public async Task<List<Atracao>> CarregarFotoAtracao()
        {
            var ListaFoto = new List<Atracao>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/CarregarFoto");
                ListaFoto = await _requestProvider.GetAsync<List<Atracao>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaFoto;
        }

        public async Task<List<Hotel>> CarregarFotoHotel()
        {
            var ListaFoto = new List<Hotel>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/CarregarFoto");
                ListaFoto = await _requestProvider.GetAsync<List<Hotel>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaFoto;
        }

        public async Task<List<Refeicao>> CarregarFotoRefeicao()
        {
            var ListaFoto = new List<Refeicao>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/CarregarFoto");
                ListaFoto = await _requestProvider.GetAsync<List<Refeicao>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return ListaFoto;
        }

        public async Task<List<Timeline>> ConsultarTimeline(CriterioBusca criterioBusca)
        {
            var Lista = new List<Timeline>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ConsultarTimeline?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<Timeline>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<Foto> CarregarFoto(int? Identificador)
        {
            Foto itemFoto = new Foto();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Foto/Get/{Identificador}");

                itemFoto = await _requestProvider.GetAsync<Foto>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }

            return itemFoto;
        }

        public async Task<List<PontoMapa>> ListarPontosViagem(CriterioBusca criterioBusca)
        {
            var Lista = new List<PontoMapa>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarPontosViagem?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<PontoMapa>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

        public async Task<List<LinhaMapa>> ListarLinhasViagem(CriterioBusca criterioBusca)
        {
            var Lista = new List<LinhaMapa>();
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.BaseEndpoint, $"Api/Consulta/ListarLinhasViagem?json={JsonConvert.SerializeObject(criterioBusca, jsonSerializerSettings)}");
                Lista = await _requestProvider.GetAsync<List<LinhaMapa>>(uri, GlobalSetting.Instance.AuthToken);

            }
            catch (Exception ex)
            {
                await ExibirMensagemErro(ex);
            }
            return Lista;
        }

    }
}
