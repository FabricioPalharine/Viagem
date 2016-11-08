using System.Collections.Generic;
using CV.Business.Library;
using CV.Data;

using CV.Model;
using CV.Model.Results;
using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

namespace CV.Business

{
    public partial class ViagemBusiness : BusinessBase
    {
        private void ValidarRegrasNegocioAluguelGasto(AluguelGasto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAluguelGasto(AluguelGasto itemGravar)
        {
        }
        private void ValidarRegrasNegocioReabastecimento(Reabastecimento itemGravar)
        {
        }
        private void ValidarRegrasExclusaoReabastecimento(Reabastecimento itemGravar)
        {
        }
        private void ValidarRegrasNegocioAmigo(Amigo itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAmigo(Amigo itemGravar)
        {
        }
        private void ValidarRegrasNegocioAporteDinheiro(AporteDinheiro itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAporteDinheiro(AporteDinheiro itemGravar)
        {
        }
        private void ValidarRegrasNegocioAtracao(Atracao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAtracao(Atracao itemGravar)
        {
        }
        private void ValidarRegrasNegocioAvaliacaoAerea(AvaliacaoAerea itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAvaliacaoAerea(AvaliacaoAerea itemGravar)
        {
        }
        private void ValidarRegrasNegocioAvaliacaoAluguel(AvaliacaoAluguel itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAvaliacaoAluguel(AvaliacaoAluguel itemGravar)
        {
        }
        private void ValidarRegrasNegocioAvaliacaoAtracao(AvaliacaoAtracao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAvaliacaoAtracao(AvaliacaoAtracao itemGravar)
        {
        }
        private void ValidarRegrasNegocioCalendarioPrevisto(CalendarioPrevisto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCalendarioPrevisto(CalendarioPrevisto itemGravar)
        {
        }
        private void ValidarRegrasNegocioCarro(Carro itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCarro(Carro itemGravar)
        {
        }
        private void ValidarRegrasNegocioCarroEvento(CarroEvento itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCarroEvento(CarroEvento itemGravar)
        {
        }
        private void ValidarRegrasNegocioCidade(Cidade itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCidade(Cidade itemGravar)
        {
        }
        private void ValidarRegrasNegocioCidadeGrupo(CidadeGrupo itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCidadeGrupo(CidadeGrupo itemGravar)
        {
        }
        private void ValidarRegrasNegocioComentario(Comentario itemGravar)
        {
        }
        private void ValidarRegrasExclusaoComentario(Comentario itemGravar)
        {
        }
        private void ValidarRegrasNegocioCotacaoMoeda(CotacaoMoeda itemGravar)
        {
        }
        private void ValidarRegrasExclusaoCotacaoMoeda(CotacaoMoeda itemGravar)
        {
        }
        private void ValidarRegrasNegocioFoto(Foto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoFoto(Foto itemGravar)
        {
        }
        private void ValidarRegrasNegocioFotoAtracao(FotoAtracao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoFotoAtracao(FotoAtracao itemGravar)
        {
        }
        private void ValidarRegrasNegocioFotoHotel(FotoHotel itemGravar)
        {
        }
        private void ValidarRegrasExclusaoFotoHotel(FotoHotel itemGravar)
        {
        }
        private void ValidarRegrasNegocioFotoItemCompra(FotoItemCompra itemGravar)
        {
        }
        private void ValidarRegrasExclusaoFotoItemCompra(FotoItemCompra itemGravar)
        {
        }
        private void ValidarRegrasNegocioFotoRefeicao(FotoRefeicao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoFotoRefeicao(FotoRefeicao itemGravar)
        {
        }
        private void ValidarRegrasNegocioGasto(Gasto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGasto(Gasto itemGravar)
        {
        }
        private void ValidarRegrasNegocioGastoCompra(GastoCompra itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoCompra(GastoCompra itemGravar)
        {
        }
        private void ValidarRegrasNegocioGastoHotel(GastoHotel itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoHotel(GastoHotel itemGravar)
        {
        }
        private void ValidarRegrasNegocioGastoPosicao(GastoPosicao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoPosicao(GastoPosicao itemGravar)
        {
        }
        private void ValidarRegrasNegocioGastoRefeicao(GastoRefeicao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoRefeicao(GastoRefeicao itemGravar)
        {
        }
        private void ValidarRegrasNegocioGastoViagemAerea(GastoViagemAerea itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoViagemAerea(GastoViagemAerea itemGravar)
        {
        }
        private void ValidarRegrasNegocioHotel(Hotel itemGravar)
        {
        }
        private void ValidarRegrasExclusaoHotel(Hotel itemGravar)
        {
        }
        private void ValidarRegrasNegocioHotelAvaliacao(HotelAvaliacao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoHotelAvaliacao(HotelAvaliacao itemGravar)
        {
        }
        private void ValidarRegrasNegocioItemCompra(ItemCompra itemGravar)
        {
        }
        private void ValidarRegrasExclusaoItemCompra(ItemCompra itemGravar)
        {
        }
        private void ValidarRegrasNegocioListaCompra(ListaCompra itemGravar)
        {
        }
        private void ValidarRegrasExclusaoListaCompra(ListaCompra itemGravar)
        {
        }
        private void ValidarRegrasNegocioLoja(Loja itemGravar)
        {
        }
        private void ValidarRegrasExclusaoLoja(Loja itemGravar)
        {
        }
        private void ValidarRegrasNegocioPais(Pais itemGravar)
        {
        }
        private void ValidarRegrasExclusaoPais(Pais itemGravar)
        {
        }
        private void ValidarRegrasNegocioParticipanteViagem(ParticipanteViagem itemGravar)
        {
        }
        private void ValidarRegrasExclusaoParticipanteViagem(ParticipanteViagem itemGravar)
        {
        }
        private void ValidarRegrasNegocioPosicao(Posicao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoPosicao(Posicao itemGravar)
        {
        }
        private void ValidarRegrasNegocioReabastecimentoGasto(ReabastecimentoGasto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoReabastecimentoGasto(ReabastecimentoGasto itemGravar)
        {
        }
        private void ValidarRegrasNegocioRefeicao(Refeicao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoRefeicao(Refeicao itemGravar)
        {
        }
        private void ValidarRegrasNegocioRequisicaoAmizade(RequisicaoAmizade itemGravar)
        {
        }
        private void ValidarRegrasExclusaoRequisicaoAmizade(RequisicaoAmizade itemGravar)
        {
        }
        private void ValidarRegrasNegocioSugestao(Sugestao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoSugestao(Sugestao itemGravar)
        {
        }
        private void ValidarRegrasNegocioUsuario(Usuario itemGravar)
        {
        }
        private void ValidarRegrasExclusaoUsuario(Usuario itemGravar)
        {
        }
        private void ValidarRegrasNegocioViagem(Viagem itemGravar)
        {
        }
        private void ValidarRegrasExclusaoViagem(Viagem itemGravar)
        {
        }
        private void ValidarRegrasNegocioViagemAerea(ViagemAerea itemGravar)
        {
        }
        private void ValidarRegrasExclusaoViagemAerea(ViagemAerea itemGravar)
        {
        }
        private void ValidarRegrasNegocioViagemAereaAeroporto(ViagemAereaAeroporto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoViagemAereaAeroporto(ViagemAereaAeroporto itemGravar)
        {
        }
        private void ValidarRegrasNegocioRefeicaoPedido(RefeicaoPedido itemGravar)
        {
        }
        private void ValidarRegrasExclusaoRefeicaoPedido(RefeicaoPedido itemGravar)
        {
        }
        private void ValidarRegrasNegocioAvaliacaoLoja(AvaliacaoLoja itemGravar)
        {
        }
        private void ValidarRegrasExclusaoAvaliacaoLoja(AvaliacaoLoja itemGravar)
        {
        }
        private void ValidarRegrasNegocioUsuarioGasto(UsuarioGasto itemGravar)
        {
        }
        private void ValidarRegrasExclusaoUsuarioGasto(UsuarioGasto itemGravar)
        {
        }
        private void ValidarRegrasNegocioHotelEvento(HotelEvento itemGravar)
        {
        }
        private void ValidarRegrasExclusaoHotelEvento(HotelEvento itemGravar)
        {
        }
        public List<Usuario> ListarUsuario(Expression<Func<Usuario, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarUsuario(predicate);
            }

        }

        public List<RequisicaoAmizade> ListarRequisicaoAmizade(Expression<Func<RequisicaoAmizade, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarRequisicaoAmizade(predicate);
            }

        }

        public List<ParticipanteViagem> ListarParticipanteViagem(Expression<Func<ParticipanteViagem, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarParticipanteViagem(predicate);
            }

        }

        public List<Amigo> ListarAmigo(Expression<Func<Amigo, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarAmigo(predicate);
            }

        }

        public List<UsuarioGasto> ListarUsuarioGasto(Expression<Func<UsuarioGasto, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarUsuarioGasto(predicate);
            }
        }

        public void SalvarUsuario(Usuario itemGravar, List<ParticipanteViagem> Participantes, List<RequisicaoAmizade> Requisicoes, List<Amigo> Amigos)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioUsuario(itemGravar);
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarUsuario(itemGravar, Requisicoes, Participantes, Amigos);

                }
            }
        }

        public UsuarioLogado CarregarDadosAplicativo(UsuarioLogado itemLogin)
        {
            UsuarioLogado itemResultado = new UsuarioLogado();
            var ItemUsuario = ListarUsuario(d => d.Codigo == itemLogin.CodigoGoogle).FirstOrDefault();
            if (ItemUsuario != null)
            {
                WebClient client = new WebClient();

                if (ItemUsuario.DataToken.GetValueOrDefault().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now)
                {
                    // creates the post data for the POST request
                    System.Collections.Specialized.NameValueCollection values = new System.Collections.Specialized.NameValueCollection();
                    values.Add("client_id", "210037759249.apps.googleusercontent.com");
                    values.Add("client_secret", "H1CNNlmDu-uNnGll5ylQmvgp");
                    values.Add("refresh_token", ItemUsuario.RefreshToken);
                    values.Add("grant_type", "refresh_token");

                    //try
                    //{
                    byte[] response = client.UploadValues("https://accounts.google.com/o/oauth2/token", values);

                    string TokenInformation = System.Text.Encoding.UTF8.GetString(response);

                    var retornoCompleto = JsonConvert.DeserializeObject<dynamic>(TokenInformation);

                    ItemUsuario.Token = retornoCompleto.access_token;
                    ItemUsuario.Lifetime = retornoCompleto.expires_in;
                    SalvarUsuario(ItemUsuario);
                }

                string UserInformation = client.DownloadString("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + ItemUsuario.Token);
                var userinfo = JsonConvert.DeserializeObject<dynamic>(UserInformation);

                AuthenticationToken token = new AuthenticationToken();
                token.Cultura = "pt-br";
                token.Token = ItemUsuario.Token;
                token.IdentificadorUsuario = ItemUsuario.Identificador.GetValueOrDefault();
                string Texto = JsonConvert.SerializeObject(token);
                Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
                itemResultado.AuthenticationToken = Texto;
                itemResultado.Email = ItemUsuario.EMail;
                itemResultado.Nome = ItemUsuario.Nome;
                itemResultado.LinkFoto = userinfo.picture;
                itemResultado.CodigoGoogle = ItemUsuario.Codigo;
            }
            return itemResultado;
        }


        public UsuarioLogado LoginAplicativo(DadosGoogleToken itemToken)
        {
            UsuarioLogado itemResultado = new UsuarioLogado();

            WebClient client = new WebClient();


            string TokenInformation = client.DownloadString("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + itemToken.access_token);
            //string TokenInformation = System.Text.Encoding.UTF8.GetString(response);
            var userinfo = JsonConvert.DeserializeObject<dynamic>(TokenInformation);
            PocoLogin itemLogin = new PocoLogin() { EMail = userinfo.email, Nome = userinfo.name, Codigo = userinfo.id };
            var ItemUsuario = ListarUsuario(d => d.Codigo == itemLogin.Codigo).FirstOrDefault();
            ItemUsuario = CadastrarUsuario(itemLogin, new ViagemBusiness(), ItemUsuario, itemToken.access_token, itemToken.refresh_token, Convert.ToInt32(itemToken.expires_in));

            AuthenticationToken token = new AuthenticationToken();
            token.Cultura = "pt-br";
            token.Token = ItemUsuario.Token;
            token.IdentificadorUsuario = ItemUsuario.Identificador.GetValueOrDefault();
            string Texto = JsonConvert.SerializeObject(token);
            Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
            itemResultado.AuthenticationToken = Texto;
            itemResultado.Email = itemLogin.EMail;
            itemResultado.Nome = itemLogin.Nome;
            itemResultado.LinkFoto = userinfo.picture;
            itemResultado.CodigoGoogle = itemLogin.Codigo;
            itemResultado.Codigo = ItemUsuario.Identificador.GetValueOrDefault();
            return itemResultado;


        }

        public UsuarioLogado LogarUsuarioGoogle(PocoLogin itemLogin, ViagemBusiness biz)
        {
            var ItemUsuario = biz.ListarUsuario(d => d.Codigo == itemLogin.Codigo).FirstOrDefault();
            UsuarioLogado itemResultado = new UsuarioLogado();

            WebClient client = new WebClient();

            // creates the post data for the POST request
            System.Collections.Specialized.NameValueCollection values = new System.Collections.Specialized.NameValueCollection();
            values.Add("client_id", "210037759249.apps.googleusercontent.com");
            values.Add("client_secret", "H1CNNlmDu-uNnGll5ylQmvgp");
            values.Add("redirect_uri", "postmessage");
            values.Add("code", itemLogin.CodigoValidacao);
            values.Add("grant_type", "authorization_code");

            //try
            //{
            byte[] response = client.UploadValues("https://accounts.google.com/o/oauth2/token", values);

            string TokenInformation = System.Text.Encoding.UTF8.GetString(response);

            var retornoCompleto = JsonConvert.DeserializeObject<dynamic>(TokenInformation);
            ItemUsuario = CadastrarUsuario(itemLogin, biz, ItemUsuario, Convert.ToString(retornoCompleto.access_token), Convert.ToString(retornoCompleto.refresh_token), Convert.ToInt32(retornoCompleto.expires_in));



            //}
            //catch (WebException ex)
            //{
            //    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
            //    string a = sr.ReadToEnd();

            //}

            itemResultado.Nome = ItemUsuario.Nome;
            itemResultado.Sucesso = true;
            itemResultado.Codigo = ItemUsuario.Identificador.GetValueOrDefault();
            itemResultado.Cultura = "pt-br";

            CarregarViagem(itemResultado, itemLogin.IdentificadorViagem);

            AuthenticationToken token = new AuthenticationToken();
            token.Cultura = "pt-br";
            token.Token = ItemUsuario.Token;
            token.IdentificadorUsuario = ItemUsuario.Identificador.GetValueOrDefault();
            token.IdentificadorViagem = itemResultado.IdentificadorViagem;
            string Texto = JsonConvert.SerializeObject(token);
            Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
            itemResultado.AuthenticationToken = Texto;

            return itemResultado;
        }

        private Usuario CadastrarUsuario(PocoLogin itemLogin, ViagemBusiness biz, Usuario ItemUsuario, string access_token, string refresh_token, int expires_in)
        {
            List<RequisicaoAmizade> requisicoesAjustadas = new List<RequisicaoAmizade>();
            List<ParticipanteViagem> participantes = new List<ParticipanteViagem>();
            List<Amigo> amigos = new List<Amigo>();

            if (ItemUsuario == null)
            {
                ItemUsuario = new Usuario();
                ItemUsuario.Codigo = itemLogin.Codigo;
                ItemUsuario.EMail = itemLogin.EMail;
                ItemUsuario.Nome = itemLogin.Nome;
                foreach (RequisicaoAmizade itemAmizade in ListarRequisicaoAmizade(d => d.EMail == itemLogin.EMail && !d.IdentificadorUsuarioRequisitado.HasValue))
                {
                    itemAmizade.IdentificadorUsuarioRequisitado = ItemUsuario.Identificador;
                    requisicoesAjustadas.Add(itemAmizade);
                }
               
                foreach (Amigo itemParticipante in ListarAmigo(d => d.EMail == itemLogin.EMail && !d.IdentificadorAmigo.HasValue))
                {
                    itemParticipante.IdentificadorUsuario = ItemUsuario.Identificador;
                    amigos.Add(itemParticipante);
                }
            }
            ItemUsuario.DataToken = DateTime.Now;
            ItemUsuario.Token = access_token;
            ItemUsuario.RefreshToken = refresh_token;
            ItemUsuario.Lifetime = expires_in;
            biz.SalvarUsuario(ItemUsuario, participantes, requisicoesAjustadas, amigos);
            return ItemUsuario;
        }

        public void CarregarViagem(UsuarioLogado itemResultado, int? IdentificadorViagem)
        {
            if (IdentificadorViagem.HasValue)
            {
                var itemViagem = SelecionarViagem_IdentificadorUsuario_IdentficadorViagem(itemResultado.Codigo, IdentificadorViagem);
                if (itemViagem != null)
                {
                    itemResultado.IdentificadorViagem = IdentificadorViagem;

                    itemResultado.NomeViagem = itemViagem.Nome;
                    itemResultado.PermiteEdicao = itemViagem.IdentificadorUsuario == itemResultado.Codigo || ListarParticipanteViagem(d => d.IdentificadorUsuario == itemResultado.Codigo).Any();
                    itemResultado.VerCustos = itemResultado.PermiteEdicao || ListarUsuarioGasto(d => d.IdentificadorUsuario == itemResultado.Codigo).Any();
                }
            }
            itemResultado.Viagens = ListarViagem(itemResultado.Codigo, true);
        }

        public List<Viagem> ListarViagem(int? IdentificadorUsuario, bool Ativa)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarViagem(IdentificadorUsuario, Ativa);
            }
        }

        public Viagem SelecionarViagem_IdentificadorUsuario_IdentficadorViagem(int? IdentificadorUsuario, int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.SelecionarViagem_IdentificadorUsuario_IdentficadorViagem(IdentificadorUsuario, IdentificadorViagem);
            }
        }

        public List<AlertaUsuario> CarregarAlertasUsuario(AuthenticationToken token)
        {
            List<AlertaUsuario> alertas = new List<AlertaUsuario>();
            List<RequisicaoAmizade> requisicoes = ListarRequisicaoAmizade(d => d.IdentificadorUsuarioRequisitado == token.IdentificadorUsuario && d.Status == 0);
            foreach (RequisicaoAmizade itemRequisicao in requisicoes)
            {
                itemRequisicao.Status = 1;
                SalvarRequisicaoAmizade(itemRequisicao);
                alertas.Add(new AlertaUsuario() { IdentificadorAlerta = itemRequisicao.Identificador.Value, MensagemAlerta = MensagemBusiness.RetornaMensagens("AlertaRequisicaoAmizade", new string[] { itemRequisicao.ItemUsuario.Nome }), TipoAlerta = 1 });
            }


            return alertas;
        }

        public List<ConsultaAmigo> ListarConsultaAmigo(int IdentificadorUsuario)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarConsultaAmigo(IdentificadorUsuario);
            }
        }



        public int? AjustarAmigo(ConsultaAmigo itemAmigo, int IdentificadorUsuario)
        {
            int? IdentificadorRequisicao = null;
            ViagemBusiness biz = new ViagemBusiness();
            if (itemAmigo.Acao == 1)
            {
                var itemBanco = ListarAmigo(d => ((itemAmigo.IdentificadorUsuario.HasValue && d.IdentificadorAmigo == itemAmigo.IdentificadorUsuario) || (!itemAmigo.IdentificadorUsuario.HasValue && d.EMail == itemAmigo.EMail)) && d.IdentificadorUsuario == IdentificadorUsuario).FirstOrDefault();
                if (itemBanco == null)
                {
                    itemBanco = new Amigo() { EMail = itemAmigo.EMail, IdentificadorAmigo = itemAmigo.IdentificadorUsuario, IdentificadorUsuario = IdentificadorUsuario };
                    biz.SalvarAmigo(itemBanco);
                }
                AdicionaMensagemBusiness("AmigoAdicionado");

            }
            else if (itemAmigo.Acao == 2)
            {
                var itemBanco = ListarAmigo(d => ((itemAmigo.IdentificadorUsuario.HasValue && d.IdentificadorAmigo == itemAmigo.IdentificadorUsuario) || (!itemAmigo.IdentificadorUsuario.HasValue && d.EMail == itemAmigo.EMail)) && d.IdentificadorUsuario == IdentificadorUsuario).FirstOrDefault();
                if (itemBanco != null)
                {
                    biz.ExcluirAmigo(itemBanco);
                }
                AdicionaMensagemBusiness("AmigoRemovido");
            }
            else if (itemAmigo.Acao == 3)
            {
                var itemBanco = ListarAmigo(d => d.IdentificadorUsuario == itemAmigo.IdentificadorUsuario && d.IdentificadorAmigo == IdentificadorUsuario).FirstOrDefault();
                if (itemBanco == null)
                {
                    var itemSolicitacao = ListarRequisicaoAmizade(d => d.IdentificadorUsuario == IdentificadorUsuario && ((d.EMail == itemAmigo.EMail && !itemAmigo.IdentificadorUsuario.HasValue) || (itemAmigo.IdentificadorUsuario.HasValue && d.IdentificadorUsuarioRequisitado == itemAmigo.IdentificadorUsuario)) && d.Status <= 1).FirstOrDefault();
                    if (itemSolicitacao == null)
                    {
                        itemSolicitacao = new RequisicaoAmizade() { EMail = itemAmigo.EMail, IdentificadorUsuario = IdentificadorUsuario, IdentificadorUsuarioRequisitado = itemAmigo.IdentificadorUsuario, Status = 0 };
                        biz.SalvarRequisicaoAmizade(itemSolicitacao);
                        IdentificadorRequisicao = itemSolicitacao.Identificador;
                    }
                }
                AdicionaMensagemBusiness("RequisicaoAmizade");
            }
            else if (itemAmigo.Acao == 4)
            {
                var itemBanco = ListarAmigo(d => d.IdentificadorUsuario == itemAmigo.IdentificadorUsuario && d.IdentificadorAmigo == IdentificadorUsuario).FirstOrDefault();
                if (itemBanco != null)
                {
                    biz.ExcluirAmigo(itemBanco);
                }
                AdicionaMensagemBusiness("AbandonarSeguido");
            }
            return IdentificadorRequisicao;
        }

        public void SalvarRequisicaoAmizadeAprovacao(RequisicaoAmizade itemRequisicao)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Amigo itemAmigo = null;
            if (itemRequisicao.Status == 2 && !biz.ListarAmigo(d => d.IdentificadorAmigo == itemRequisicao.IdentificadorUsuario && d.IdentificadorUsuario == itemRequisicao.IdentificadorUsuarioRequisitado).Any())
            {
                itemAmigo = new Amigo() { EMail = itemRequisicao.EMail, IdentificadorAmigo = itemRequisicao.IdentificadorUsuario, IdentificadorUsuario = itemRequisicao.IdentificadorUsuarioRequisitado };
                AdicionaMensagemBusiness("Requisicao_Aprovada");
            }
            else
                AdicionaMensagemBusiness("Requisicao_Reprovada");
            using (ViagemRepository data = new ViagemRepository())
            {
                data.SalvarRequisicaoAmizade(itemRequisicao, itemAmigo);
            }
        }

        public int? SalvarAmigo(ConsultaAmigo itemAmigo, int? IdentificadorUsuario)
        {
            ViagemBusiness biz = new ViagemBusiness();
            int? IdentificadorRequisicao = null;
            if (string.IsNullOrEmpty(itemAmigo.EMail))
                AdicionaErroBusiness("Amigo_MailObrigatorio", "EMail");
            else if (!Regex.IsMatch(itemAmigo.EMail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                AdicionaErroBusiness("Amigo_MailInvalido", "EMail");
            }
            if (itemAmigo.IdentificadorUsuario == IdentificadorUsuario)
                AdicionaErroBusiness("Amigo_UsuarioAtual", "EMail");
            if (!itemAmigo.Seguido && !itemAmigo.Seguidor)
                AdicionaErroBusiness("Amigo_TipoAmizade");
            if (IsValid())
            {
                if (itemAmigo.Seguidor)
                {
                    var itemBanco = ListarAmigo(d => ((itemAmigo.IdentificadorUsuario.HasValue && d.IdentificadorAmigo == itemAmigo.IdentificadorUsuario) || (!itemAmigo.IdentificadorUsuario.HasValue && d.EMail == itemAmigo.EMail)) && d.IdentificadorUsuario == IdentificadorUsuario).FirstOrDefault();

                    if (itemBanco == null)
                    {
                        itemBanco = new Amigo() { EMail = itemAmigo.EMail, IdentificadorAmigo = itemAmigo.IdentificadorUsuario, IdentificadorUsuario = IdentificadorUsuario };
                        biz.SalvarAmigo(itemBanco);
                    }
                    AdicionaMensagemBusiness("AmigoAdicionado");
                }
                if (itemAmigo.Seguido)
                {
                    var itemBanco = ListarAmigo(d => d.IdentificadorUsuario == itemAmigo.IdentificadorUsuario && d.IdentificadorAmigo == IdentificadorUsuario).FirstOrDefault();
                    if (itemBanco == null)
                    {
                        var itemSolicitacao = ListarRequisicaoAmizade(d => d.IdentificadorUsuario == IdentificadorUsuario && ((d.EMail == itemAmigo.EMail && !itemAmigo.IdentificadorUsuario.HasValue) || (itemAmigo.IdentificadorUsuario.HasValue && d.IdentificadorUsuarioRequisitado == itemAmigo.IdentificadorUsuario)) && d.Status <= 1).FirstOrDefault();
                        if (itemSolicitacao == null)
                        {
                            itemSolicitacao = new RequisicaoAmizade() { EMail = itemAmigo.EMail, IdentificadorUsuario = IdentificadorUsuario, IdentificadorUsuarioRequisitado = itemAmigo.IdentificadorUsuario, Status = 0 };
                            biz.SalvarRequisicaoAmizade(itemSolicitacao);
                            IdentificadorRequisicao = itemSolicitacao.Identificador;
                        }
                    }
                    AdicionaMensagemBusiness("RequisicaoAmizade");

                }

            }
            return IdentificadorRequisicao;
        }

        public List<Usuario> ListarUsuario_EMail(string EMail)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarUsuario_EMail(EMail);
            }
        }


        public List<Usuario> ListarUsuarioAmigo(int identificadorUsuario)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarUsuarioAmigo(identificadorUsuario);
            }
        }
    }

}
