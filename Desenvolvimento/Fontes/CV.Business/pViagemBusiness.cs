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
using System.IO;

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
            if (IsValid())
            {
                foreach (var itemGasto in itemGravar.Gastos)
                {
                    ValidateService(itemGasto.ItemGasto);
                }
            }
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
            if (itemGravar.ItemGasto != null)
            {
                if (!itemGravar.ItemGasto.Moeda.HasValue)
                    AdicionaErroBusiness("AporteDinheiro_MoedaGasto", "MoedaGasto");
                if (!itemGravar.ItemGasto.Valor.HasValue)
                    AdicionaErroBusiness("AporteDinheiro_ValorGasto", "ValorGasto");
            }
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
            if (IsValid())
            {

                ValidateService(itemGravar.ItemGasto);

            }
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
            if (itemGravar.IdentificadorUsuario.HasValue && itemGravar.IdentificadorViagem.HasValue)
            {
                ViagemBusiness biz = new ViagemBusiness();
                if (!itemGravar.IdentificadorUsuarioPedido.HasValue)
                {
                    var lista = biz.CarregarParticipantesViagem(itemGravar.IdentificadorViagem);
                    if (!lista.Where(d => d.Identificador == itemGravar.IdentificadorUsuario).Any())
                    {
                        AdicionaErroBusiness("PedidoCompra_PedirParaObrigatorio", "IdentificadorUsuarioPedido");
                    }
                }
            }
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
            foreach (var itemAeroporto in itemGravar.Aeroportos)
            {
                ValidateService(itemAeroporto);
            }
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

        private void ValidarRegrasNegocioGastoAtracao(GastoAtracao itemGravar)
        {
        }
        private void ValidarRegrasExclusaoGastoAtracao(GastoAtracao itemGravar)
        {
        }

        private void ValidarRegrasExclusaoGastoDividido(GastoDividido itemGravar)
        {
        }

        private void ValidarRegrasNegocioGastoDividido(GastoDividido itemGravar)
        {
        }

        private void ValidarRegrasNegocioCarroDeslocamento(CarroDeslocamento itemGravar)
        {

        }

        private void ValidarRegrasExclusaoCarroDeslocamento(CarroDeslocamento itemGravar)
        {

        }
        #region Save

        public void SalvarCidadeGrupo(ManutencaoCidadeGrupo itemManutencaoCidadeGrupo, int? IdentificadorViagem)
        {
            ViagemBusiness biz = new ViagemBusiness();

            if (!itemManutencaoCidadeGrupo.IdentificadorCidade.HasValue)
                AdicionaErroBusiness("CidadeGrupo_CidadePai", "IdentificadorCidade");
            if (itemManutencaoCidadeGrupo.CidadesFilhas == null || !itemManutencaoCidadeGrupo.CidadesFilhas.Any())
                AdicionaErroBusiness("CidadeGrupo_CidadeFilhaAssociada", "CidadesFilhas");
            if (!itemManutencaoCidadeGrupo.Edicao && IsValid())
            {
                if (biz.ListarCidadeGrupo_IdentificadorCidadePai(itemManutencaoCidadeGrupo.IdentificadorCidade, IdentificadorViagem).Any())
                    AdicionaErroBusiness("CidadeGrupo_CidadePaiUtilizada", "IdentificadorCidade");
            }
            if (IsValid())
            {
                var CidadesAtuais = biz.ListarCidadeGrupo_IdentificadorCidadePai(itemManutencaoCidadeGrupo.IdentificadorCidade, IdentificadorViagem).ToList();
                var CidadesExcluidas = new List<CidadeGrupo>();
                var CidadesNovas = new List<CidadeGrupo>();
                foreach (var itemCidade in CidadesAtuais.ToList())
                {
                    if (!itemManutencaoCidadeGrupo.CidadesFilhas.Contains(itemCidade.Identificador))
                    {
                        CidadesExcluidas.Add(itemCidade);
                        CidadesAtuais.Remove(itemCidade);
                    }
                }
                foreach (var identificadorCidade in itemManutencaoCidadeGrupo.CidadesFilhas)
                {
                    if (!CidadesAtuais.Where(d=>d.IdentificadorCidadeFilha == identificadorCidade).Any())
                    {
                        var itemCidadeGrupo = new CidadeGrupo() { DataAtualizacao = DateTime.Now, IdentificadorCidadeFilha = identificadorCidade, IdentificadorCidadePai = itemManutencaoCidadeGrupo.IdentificadorCidade, IdentificadorViagem = IdentificadorViagem };
                        CidadesNovas.Add(itemCidadeGrupo);
                    }
                }
                biz.ExcluirCidadeGrupo_Lista(CidadesExcluidas);
                SalvarCidadeGrupo_Lista(CidadesNovas);

            }
        }

        public void ExcluirCidadeGrupo_Lista(List<CidadeGrupo> ListaGrupos)
        {
            LimparValidacao();
            foreach (var itemGravar in ListaGrupos)
            {
                ValidateService(itemGravar);
                ValidarRegrasExclusaoCidadeGrupo(itemGravar);
            }
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.ExcluirCidadeGrupo_Lista(ListaGrupos);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_ExcluirCidadeGrupo_Lista_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }

        public void SalvarAporteDinheiroCompleto(AporteDinheiro itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioAporteDinheiro(itemGravar);
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarAporteDinheiroCompleto(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarAporteDinheiro_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }
        public void SalvarCarro_Completo(Carro itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioCarro(itemGravar);
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarCarro_Completo(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCarro_Completo_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }


        public void SalvarCarro_Evento(Carro itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioCarro(itemGravar);

            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarCarro_Evento(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCarro_Completo_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }


        public void SalvarCarroDeslocamento_Evento(CarroDeslocamento itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioCarroDeslocamento(itemGravar);
            if (itemGravar.ItemCarroEventoChegada != null)
            {
                if (itemGravar.ItemCarroEventoChegada.Data.HasValue && (!itemGravar.ItemCarroEventoChegada.Latitude.HasValue || !itemGravar.ItemCarroEventoChegada.Longitude.HasValue))
                    AdicionaErroBusiness("CarroDeslocamento_PosicaoChegada_Obrigatoria", "ItemCarroEventoChegada_Posicao");
            }
            if (itemGravar.ItemCarroEventoPartida != null)
            {
                if (!itemGravar.ItemCarroEventoPartida.Data.HasValue)
                    AdicionaErroBusiness("CarroDeslocamento_DataPartida_Obrigatoria", "ItemCarroEventoPartida_Data");

                if (itemGravar.ItemCarroEventoPartida.Data.HasValue && (!itemGravar.ItemCarroEventoPartida.Latitude.HasValue || !itemGravar.ItemCarroEventoPartida.Longitude.HasValue))
                    AdicionaErroBusiness("CarroDeslocamento_PosicaoPartida_Obrigatoria", "ItemCarroEventoPartida_Posicao");

            }
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarCarroDeslocamento_Evento(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarCarroDeslocamento_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }

        public void SalvarGastoCompra_Completo(GastoCompra itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioGastoCompra(itemGravar);
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarGastoCompra_Completo(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarGastoCompra_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }
        #endregion

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

        public List<Pais> ListarPais(Expression<Func<Pais, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarPais(predicate);
            }
        }

        public List<Cidade> ListarCidade(Expression<Func<Cidade, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarCidade(predicate);
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
                AtualizarTokenUsuario(ItemUsuario);
                //VerificarAlbum(ItemUsuario);
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
                itemResultado.Codigo = ItemUsuario.Identificador.GetValueOrDefault();
            }
            return itemResultado;
        }

        public void AtualizarTokenUsuario(Usuario ItemUsuario)
        {
            if (!ItemUsuario.DataToken.HasValue || ItemUsuario.DataToken.GetValueOrDefault().AddSeconds(ItemUsuario.Lifetime.GetValueOrDefault(0) - 60) < DateTime.Now.ToUniversalTime())
            {
                WebClient client = new WebClient();
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
                ItemUsuario.DataToken = DateTime.Now.ToUniversalTime();
                SalvarUsuario(ItemUsuario);
            }
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
            //VerificarAlbum(ItemUsuario);
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

            //VerificarAlbum(ItemUsuario);

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
            ItemUsuario.DataToken = DateTime.Now.ToUniversalTime();
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
                    itemResultado.Aberto = itemViagem.Aberto.GetValueOrDefault();
                    itemResultado.NomeViagem = itemViagem.Nome;
                    itemResultado.PermiteEdicao = itemViagem.IdentificadorUsuario == itemResultado.Codigo || ListarParticipanteViagem(d => d.IdentificadorViagem == IdentificadorViagem && d.IdentificadorUsuario == itemResultado.Codigo).Any();
                    itemResultado.VerCustos = itemResultado.PermiteEdicao || ListarUsuarioGasto(d => d.IdentificadorUsuario == itemResultado.Codigo && d.IdentificadorUsuario == itemResultado.Codigo).Any();
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
                alertas.Add(new AlertaUsuario() { IdentificadorAlerta = itemRequisicao.Identificador.Value, MensagemAlerta = MensagemBusiness.RetornaMensagens("AlertaRequisicaoAmizade", new string[] { itemRequisicao.ItemUsuario.Nome }), TipoAlerta = 1 });
            }
            if (token.IdentificadorViagem.HasValue)
            {
                List<Sugestao> lista = ListarSugestao(null, token.IdentificadorViagem, null, null, null, 0);
                foreach (Sugestao itemRequisicao in lista)
                {
                    alertas.Add(new AlertaUsuario() { IdentificadorAlerta = itemRequisicao.Identificador.Value, MensagemAlerta = MensagemBusiness.RetornaMensagens("AlertaSugestaoVisita", new string[] { itemRequisicao.ItemUsuario.Nome, itemRequisicao.Local }), TipoAlerta = 2 });
                }
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
            if (itemRequisicao.Status == 2 )
            {
                if( !biz.ListarAmigo(d => d.IdentificadorAmigo == itemRequisicao.IdentificadorUsuario && d.IdentificadorUsuario == itemRequisicao.IdentificadorUsuarioRequisitado).Any())
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


        public List<Usuario> ListarUsuarioAmigoComigo(int identificadorUsuario)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarUsuarioAmigoComigo(identificadorUsuario);
            }
        }


        public List<Viagem> ListarViagem(int? IdentificadorParticipante, string Nome, bool? Aberto, DateTime? DataInicioDe, DateTime? DataFimDe, DateTime? DataInicioAte, DateTime? DataFimAte, int? IdentificadorUsuario)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarViagem(IdentificadorParticipante, Nome, Aberto, DataInicioDe, DataInicioAte, DataFimDe, DataFimAte, IdentificadorUsuario);
            }
        }

        public string CriarAlbum(Usuario itemUsuario, string Nome)
        {
            AtualizarTokenUsuario(itemUsuario);
            using (WebClient wc = new WebClient())
            {

                wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + itemUsuario.Token;
                wc.Headers["GData-Version"] = "2";
                //var jsonData = wc.DownloadString("https://picasaweb.google.com/data/feed/api/user/default?alt=json");
                //var albuminfo = JsonConvert.DeserializeObject<dynamic>(jsonData);
                //var entries = ((Newtonsoft.Json.Linq.JArray)albuminfo.feed.entry);
                //foreach (var itemEntry in entries)
                //{
                //    if (itemEntry["title"].First.First.ToString() == "CurtindoViagem")
                //    {
                //        return itemEntry["gphoto$id"].First.First.ToString();
                //    }
                //}

                string XMLEntrada = @"<entry xmlns='http://www.w3.org/2005/Atom'
    xmlns:media='http://search.yahoo.com/mrss/'
    xmlns:gphoto='http://schemas.google.com/photos/2007'>
  <title type='text'>" + Nome + @"</title>
  <gphoto:access>public</gphoto:access>
  <category scheme='http://schemas.google.com/g/2005#kind'
    term='http://schemas.google.com/photos/2007#album'></category>
</entry>";
                wc.Headers[HttpRequestHeader.ContentType] = "application/atom+xml";


                string URI = "https://picasaweb.google.com/data/feed/api/user/default?alt=json";
                string HtmlResult = wc.UploadString(URI, "POST", XMLEntrada);
                var newalbuminfo = JsonConvert.DeserializeObject<dynamic>(HtmlResult);
                return ((Newtonsoft.Json.Linq.JObject)newalbuminfo.entry)["gphoto$id"].First.First.ToString();
            }

        }

        public void SalvarViagem_Completa_Album(Viagem itemViagem)
        {

            ValidateService(itemViagem);
            ValidarRegrasNegocioViagem(itemViagem);
            if (IsValid())
            {
                if (string.IsNullOrEmpty(itemViagem.CodigoAlbum))
                {
                    Usuario itemUsuario = SelecionarUsuario(itemViagem.IdentificadorUsuario);
                    itemViagem.CodigoAlbum = CriarAlbum(itemUsuario, itemViagem.Nome);
                }
                SalvarViagem_Completa(itemViagem);
            }
        }

        public ResultadoOperacao CadastrarFoto(UploadFoto itemFoto, Usuario itemUsuario, int? IdentificadoViagem)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            Viagem itemViagem = SelecionarViagem(IdentificadoViagem);
            AtualizarTokenUsuario(itemUsuario);
            SubirImagemPicasa(itemFoto, itemUsuario, itemViagem);
            itemResultado.ItemRegistro = CadastrarNovaFoto(itemViagem, itemUsuario, itemFoto, false);
            return itemResultado;
        }

        public ResultadoOperacao CadastrarFotoDireto(UploadFoto itemFoto, Usuario itemUsuario, int? IdentificadoViagem)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            Viagem itemViagem = SelecionarViagem(IdentificadoViagem);
            itemResultado.ItemRegistro = CadastrarNovaFoto(itemViagem, itemUsuario, itemFoto, false);
            return itemResultado;
        }

        private void SubirImagemPicasa(UploadFoto itemFoto, Usuario itemUsuario, Viagem itemViagem)
        {
            string Album = itemViagem.CodigoAlbum;
            string Base64 = itemFoto.Base64;
            if (!string.IsNullOrEmpty(Base64))
            {
                int Posicao = Base64.IndexOf("base64,");
                if (Posicao >= 0)
                    Base64 = Base64.Substring(Posicao + 7);
            }
            byte[] Dados = Convert.FromBase64String(Base64);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://picasaweb.google.com/data/feed/api/user/default/albumid/" + Album + "?alt=json-in-script");
            request.Method = "POST";
            request.ContentType = itemFoto.ImageMime;
            request.ContentLength = Dados.Length;
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + itemUsuario.Token);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(Dados, 0, Dados.Length);

                requestStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());

            string responseStr = responseReader.ReadToEnd();
            responseStr = responseStr.Substring(0, responseStr.Length - 2).Replace("gdata.io.handleScriptLoaded(", "");

            var newimageinfo = JsonConvert.DeserializeObject<dynamic>(responseStr);

            itemFoto.CodigoGoogle = ((Newtonsoft.Json.Linq.JContainer)(newimageinfo.entry["gphoto$id"])).First.First.ToString();


            itemFoto.LinkGoogle = newimageinfo.entry["media$group"]["media$content"][0]["url"].ToString();
            itemFoto.Thumbnail = newimageinfo.entry["media$group"]["media$thumbnail"][1]["url"].ToString();
        }

        public ResultadoOperacao CadastrarVideo(UploadFoto itemFoto, Usuario itemUsuario, int? IdentificadoViagem)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            Viagem itemViagem = SelecionarViagem(IdentificadoViagem);

            itemResultado.ItemRegistro = CadastrarNovaFoto(itemViagem, itemUsuario, itemFoto, true);

            return itemResultado;
        }

        public Foto CadastrarNovaFoto(Viagem itemViagem, Usuario itemUsuario, UploadFoto itemFoto, bool Video)
        {

            Foto itemGravarFoto = new Foto();
            itemGravarFoto.IdentificadorViagem = itemViagem.Identificador;
            itemGravarFoto.Atracoes = new List<FotoAtracao>();
            itemGravarFoto.CodigoFoto = itemFoto.CodigoGoogle;
            itemGravarFoto.Data = itemFoto.DataArquivo;
            itemGravarFoto.DataAtualizacao = DateTime.Now;
            itemGravarFoto.Hoteis = new List<FotoHotel>();
            itemGravarFoto.Comentario = itemFoto.Comentario;
            itemGravarFoto.IdentificadorUsuario = itemUsuario.Identificador;
            itemGravarFoto.ItensCompra = new List<FotoItemCompra>();

            itemGravarFoto.Refeicoes = new List<FotoRefeicao>();
            itemGravarFoto.TipoArquivo = itemFoto.ImageMime;
            itemGravarFoto.Video = Video;
            itemGravarFoto.LinkFoto = itemFoto.LinkGoogle;
            itemGravarFoto.LinkThumbnail = itemFoto.Thumbnail;

            if (itemFoto.Latitude.HasValue && itemFoto.Longitude.HasValue)
            {
                itemGravarFoto.Latitude = itemFoto.Latitude;
                itemGravarFoto.Longitude = itemFoto.Longitude;
            }
            else
                LocalizarPosicaoFoto(itemViagem.Identificador, itemUsuario.Identificador, itemGravarFoto.Data, itemGravarFoto);

            if (itemGravarFoto.Latitude.HasValue && itemGravarFoto.Longitude.HasValue)
            {
                itemGravarFoto.IdentificadorCidade = RetornarCidadeGeocoding(itemGravarFoto.Latitude, itemGravarFoto.Longitude);
            }
            DetectarAssociacoesFoto(itemUsuario.Identificador, itemViagem.Identificador, itemGravarFoto, itemFoto);
            SalvarFoto_Completa(itemGravarFoto);
            itemGravarFoto = SelecionarFoto_Completa(itemGravarFoto.Identificador);
            itemGravarFoto.Atracoes.ToList().ForEach(d => { d.ItemFoto = null; d.ItemAtracao.Fotos = null; d.ItemAtracao.ItemAtracaoPai = null; });
            itemGravarFoto.Hoteis.ToList().ForEach(d => { d.ItemFoto = null; d.ItemHotel.Fotos = null; });
            itemGravarFoto.ItensCompra.ToList().ForEach(d => { d.ItemFoto = null; d.ItemItemCompra.Fotos = null; });
            itemGravarFoto.Refeicoes.ToList().ForEach(d => { d.ItemFoto = null; d.ItemRefeicao.Fotos = null; });

            return itemGravarFoto;
        }

        private void LocalizarPosicaoFoto(int? IdentificadorUsuario, int? IdentificadorViagem, DateTime? data, Foto itemGravarFoto)
        {
            DateTime? DataMinima = data.GetValueOrDefault().AddMinutes(-5);
            DateTime? DataMaxima = data.GetValueOrDefault().AddMinutes(5);
            var lista = ListarPosicao(d => d.IdentificadorViagem == IdentificadorViagem && d.IdentificadorUsuario == IdentificadorUsuario && d.DataLocal >= DataMinima && d.DataLocal <= DataMaxima);
            if (!lista.Any())
                lista = ListarPosicao(d => d.IdentificadorViagem == IdentificadorViagem && d.DataLocal >= DataMinima && d.DataLocal <= DataMaxima);
            if (lista.Any())
            {
                var itemPosicao = lista.OrderBy(d => Math.Abs(data.GetValueOrDefault().Subtract(d.DataLocal.GetValueOrDefault()).TotalSeconds)).FirstOrDefault();
                itemGravarFoto.Longitude = itemPosicao.Longitude;
                itemGravarFoto.Latitude = itemPosicao.Latitude;
            }
        }


        public List<HotelEvento> ListarHotelEvento(Expression<Func<HotelEvento, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarHotelEvento(predicate);
            }
        }

        public List<Hotel> ListarHotelData(int? IdentificadorViagem, DateTime? Data)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarHotelData(IdentificadorViagem, Data);
            }
        }

        private void LocalizarAtracoesPai(int? IdentificadorAtracao, List<int?> AtracoesPai)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Atracao itemAtracao = biz.SelecionarAtracao(IdentificadorAtracao);
            if (itemAtracao.IdentificadorAtracaoPai.HasValue && !AtracoesPai.Contains(itemAtracao.IdentificadorAtracaoPai))
            {
                AtracoesPai.Add(itemAtracao.IdentificadorAtracaoPai);
                LocalizarAtracoesPai(itemAtracao.IdentificadorAtracaoPai, AtracoesPai);

            }
        }

        private void DetectarAssociacoesFoto(int? IdentificadorUsuario, int? IdentificadorViagem, Foto itemGravarFoto, UploadFoto itemFoto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            if (!itemFoto.IdentificadorAtracao.HasValue)
            {
                var ListaAtracoes = ListarAtracao(d => d.IdentificadorViagem == IdentificadorViagem && d.Chegada <= itemGravarFoto.Data &&
                (!d.Partida.HasValue || d.Partida >= itemGravarFoto.Data) && !d.DataExclusao.HasValue);
                foreach (var itemAtracao in ListaAtracoes)
                    itemGravarFoto.Atracoes.Add(new FotoAtracao() { DataAtualizacao = DateTime.Now, IdentificadorAtracao = itemAtracao.Identificador });
            }
            else if (!itemFoto.IdentificadorRefeicao.HasValue && !itemFoto.IdentificadorHotel.HasValue)
            {
                List<int?> AtracoesPai = new List<int?>();
                AtracoesPai.Add(itemFoto.IdentificadorAtracao);
                LocalizarAtracoesPai(itemFoto.IdentificadorAtracao, AtracoesPai);
                foreach (var IdentificadorAtracao in AtracoesPai)
                    itemGravarFoto.Atracoes.Add(new FotoAtracao() { DataAtualizacao = DateTime.Now, IdentificadorAtracao = IdentificadorAtracao });




            }
            if (itemFoto.IdentificadorHotel.HasValue)
            {
                itemGravarFoto.Hoteis.Add(new FotoHotel() { DataAtualizacao = DateTime.Now, IdentificadorHotel = itemFoto.IdentificadorHotel });

            }
            else if (!itemFoto.IdentificadorRefeicao.HasValue && !itemFoto.IdentificadorAtracao.HasValue)
                foreach (var itemHotel in ListarHotelData(IdentificadorViagem, itemGravarFoto.Data))
                    itemGravarFoto.Hoteis.Add(new FotoHotel() { DataAtualizacao = DateTime.Now, IdentificadorHotel = itemHotel.Identificador });


            if (itemFoto.IdentificadorItemCompra.HasValue)
            {
                itemGravarFoto.ItensCompra.Add(new FotoItemCompra() { DataAtualizacao = DateTime.Now, IdentificadorItemCompra = itemFoto.IdentificadorItemCompra });

            }

            if (itemFoto.IdentificadorRefeicao.HasValue)
            {
                itemGravarFoto.Refeicoes.Add(new FotoRefeicao() { DataAtualizacao = DateTime.Now, IdentificadorRefeicao = itemFoto.IdentificadorRefeicao });
                Refeicao itemRefeicao = biz.SelecionarRefeicao(itemFoto.IdentificadorRefeicao);
                if (itemRefeicao.IdentificadorAtracao.HasValue)
                {
                    List<int?> AtracoesPai = new List<int?>();
                    AtracoesPai.Add(itemRefeicao.IdentificadorAtracao);
                    LocalizarAtracoesPai(itemRefeicao.IdentificadorAtracao, AtracoesPai);
                    foreach (var IdentificadorAtracao in AtracoesPai)
                        itemGravarFoto.Atracoes.Add(new FotoAtracao() { DataAtualizacao = DateTime.Now, IdentificadorAtracao = IdentificadorAtracao });
                }
            }
            else if (!itemFoto.IdentificadorHotel.HasValue && !itemFoto.IdentificadorAtracao.HasValue )
            {
                DateTime dataInicio = itemGravarFoto.Data.GetValueOrDefault().AddMinutes(-5);
                DateTime dataFim = itemGravarFoto.Data.GetValueOrDefault().AddMinutes(5);

                var itemRefeicao = ListarRefeicao(d => d.IdentificadorViagem == IdentificadorViagem && d.Data >= dataInicio && d.Data <= dataFim).FirstOrDefault();
                if (itemRefeicao != null)
                    itemGravarFoto.Refeicoes.Add(new FotoRefeicao() { DataAtualizacao = DateTime.Now, IdentificadorRefeicao = itemRefeicao.Identificador });
            }
        }

        public List<Posicao> ListarPosicao(Expression<Func<Posicao, bool>> predicate)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                return data.ListarPosicao(predicate);
            }
        }

        public int? RetornarCidadeGeocoding(decimal? latitude, decimal? longitude)
        {
            WebClient client = new WebClient();

            client.Encoding = System.Text.Encoding.UTF8;
            string LocationInformation = client.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude.GetValueOrDefault(0).ToString("F6", System.Globalization.CultureInfo.GetCultureInfo("en-Us")) + "," + longitude.GetValueOrDefault(0).ToString("F6", System.Globalization.CultureInfo.GetCultureInfo("en-Us")) + "&key=AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g");

            var locationinfo = JsonConvert.DeserializeObject<dynamic>(LocationInformation);
            int? IdentificadorCidade = null;
            if (locationinfo.status == "OK")
            {
                string Cidade = null;
                string Estado = string.Empty;
                string Pais = null;
                string SiglaPais = null;
                var entries = ((Newtonsoft.Json.Linq.JArray)locationinfo.results);
                foreach (var itemResult in entries)
                {

                    if (itemResult["types"].ToString().IndexOf("administrative_area_level_2", StringComparison.InvariantCultureIgnoreCase) >= 0
                        || itemResult["types"].ToString().IndexOf("locality", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        Cidade = itemResult["address_components"][0]["long_name"].ToString();
                    }
                    if (itemResult["types"].ToString().IndexOf("administrative_area_level_1", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        Estado = itemResult["address_components"][0]["long_name"].ToString();
                    }
                    if (itemResult["types"].ToString().IndexOf("country", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        Pais = itemResult["address_components"][0]["long_name"].ToString();
                        SiglaPais = itemResult["address_components"][0]["short_name"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Pais))
                {
                    Pais itemPais = ListarPais(d => d.Nome == Pais).FirstOrDefault();
                    if (itemPais == null)
                    {
                        itemPais = new Model.Pais();
                        itemPais.Nome = Pais;
                        itemPais.Sigla = SiglaPais;
                        SalvarPais(itemPais);
                    }
                    Cidade itemCidade = ListarCidade(d => d.IdentificadorPais == itemPais.Identificador && d.Nome == Cidade && d.Estado == Estado).FirstOrDefault();
                    if (itemCidade == null)
                    {
                        itemCidade = new Model.Cidade();
                        itemCidade.IdentificadorPais = itemPais.Identificador;
                        itemCidade.Nome = Cidade;
                        itemCidade.Estado = Estado;
                        SalvarCidade(itemCidade);
                    }
                    IdentificadorCidade = itemCidade.Identificador;

                }
            }
            return IdentificadorCidade;
        }

        public List<Foto> ListarFotos(int? IdentificadorFoto, int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Comentario, List<int?> IdentificadorAtracao,
           List<int?> IdentificadorHotel, List<int?> IdentificadorRefeicao, int? IdentificadorCidade, int Skip, int Count)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                var lista = repositorio.ListarFotos(IdentificadorFoto, IdentificadorViagem, DataDe, DataAte, Comentario, IdentificadorAtracao, IdentificadorHotel, IdentificadorRefeicao, IdentificadorCidade, Skip, Count);
                foreach (var itemFoto in lista)
                {
                    itemFoto.Atracoes.ToList().ForEach(d => { d.ItemFoto = null; d.ItemAtracao.Fotos = null; d.ItemAtracao.ItemAtracaoPai = null; });
                    itemFoto.Hoteis.ToList().ForEach(d => { d.ItemFoto = null; d.ItemHotel.Fotos = null; });
                    itemFoto.ItensCompra.ToList().ForEach(d => { d.ItemFoto = null; d.ItemItemCompra.Fotos = null; });
                    itemFoto.Refeicoes.ToList().ForEach(d => { d.ItemFoto = null; d.ItemRefeicao.Fotos = null; });

                }
                return lista;
            }
        }

        public List<Cidade> CarregarCidadeViagemFoto(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeViagemFoto(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeAtracao(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeAtracao(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeRefeicao(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeRefeicao(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeHotel(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeHotel(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeViagemAerea(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeViagemAerea(IdentificadorViagem);
            }
        }


        public List<Cidade> CarregarCidadeLoja(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeLoja(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeComentario(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeComentario(IdentificadorViagem);
            }
        }

        public List<Cidade> CarregarCidadeSugestao(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarCidadeSugestao(IdentificadorViagem);
            }
        }
        public List<Refeicao> ListarRefeicao(Expression<Func<Refeicao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarRefeicao(predicate);
            }
        }

        public List<Hotel> ListarHotel(Expression<Func<Hotel, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarHotel(predicate);
            }
        }

        public List<Atracao> ListarAtracao(Expression<Func<Atracao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAtracao(predicate);
            }
        }

        public List<ItemCompra> ListarItemCompra(Expression<Func<ItemCompra, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarItemCompra(predicate);
            }
        }
        public List<AvaliacaoAtracao> ListarAvaliacaoAtracao(Expression<Func<AvaliacaoAtracao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAvaliacaoAtracao(predicate);
            }
        }
        public List<RefeicaoPedido> ListarRefeicaoPedido(Expression<Func<RefeicaoPedido, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarRefeicaoPedido(predicate);
            }
        }

        public List<CotacaoMoeda> ListarCotacaoMoeda(Expression<Func<CotacaoMoeda, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCotacaoMoeda(predicate);
            }
        }

        public List<Comentario> ListarComentario(Expression<Func<Comentario, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarComentario(predicate);
            }
        }

        public List<AporteDinheiro> ListarAporteDinheiro(Expression<Func<AporteDinheiro, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAporteDinheiro(predicate);
            }
        }

        public List<Gasto> ListarGasto(Expression<Func<Gasto, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGasto(predicate);
            }
        }

        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, Expression<Func<ListaCompra, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarListaCompra(IdentificadorUsuario, predicate);
            }
        }

        public List<Sugestao> ListarSugestao(Expression<Func<Sugestao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarSugestao(predicate);
            }
        }

        public List<CalendarioPrevisto> ListarCalendarioPrevisto(Expression<Func<CalendarioPrevisto, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCalendarioPrevisto(predicate);
            }
        }

        public List<Atracao> ListarAtracao_Completo(Expression<Func<Atracao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAtracao_Completo(predicate);
            }
        }

        public List<Hotel> ListarHotel_Completo(Expression<Func<Hotel, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarHotel_Completo(predicate);
            }
        }

        public List<Refeicao> ListarRefeicao_Completo(Expression<Func<Refeicao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarRefeicao_Completo(predicate);
            }
        }

        public List<Loja> ListarLoja(Expression<Func<Loja, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarLoja(predicate);
            }
        }

        public List<Carro> ListarCarro(Expression<Func<Carro, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCarro(predicate);
            }
        }

        public List<ViagemAerea> ListarViagemAerea(Expression<Func<ViagemAerea, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarViagemAerea(predicate);
            }
        }

        public List<GastoCompra> ListarGastoCompra(int? IdentificadorViagem, int? IdentificadorUsuario, Expression<Func<GastoCompra, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoCompra(IdentificadorViagem,IdentificadorUsuario, predicate);
            }
        }

        public List<GastoCompra> ListarGastoCompra( Expression<Func<GastoCompra, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoCompra( predicate);
            }
        }


        public List<ReabastecimentoGasto> ListarReabastecimentoGastos(Expression<Func<ReabastecimentoGasto, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarReabastecimentoGastos(predicate);
            }
        }

        public List<Reabastecimento> ListarReabastecimento(int? IdentificadorViagem, Expression<Func<Reabastecimento, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarReabastecimento(IdentificadorViagem, predicate);
            }
        }
        public List<CarroDeslocamento> ListarCarroDeslocamento(int? IdentificadorViagem, Expression<Func<CarroDeslocamento, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCarroDeslocamento(IdentificadorViagem, predicate);
            }
        }
        public List<HotelEvento> ListarHotelEvento(int? IdentificadorViagem, Expression<Func<HotelEvento, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarHotelEvento(IdentificadorViagem, predicate);
            }
        }

        public List<ItemCompra> ListarItemCompra(int? IdentificadorViagem, Expression<Func<ItemCompra, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarItemCompra(IdentificadorViagem, predicate);
            }
        }


        public List<AluguelGasto> ListarAluguelGasto(int? IdentificadorViagem, Expression<Func<AluguelGasto, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAluguelGasto(IdentificadorViagem, predicate);
            }
        }

        public List<GastoAtracao> ListarGastoAtracao(int? IdentificadorViagem, Expression<Func<GastoAtracao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoAtracao(IdentificadorViagem, predicate);
            }
        }
        public List<GastoHotel> ListarGastoHotel(int? IdentificadorViagem, Expression<Func<GastoHotel, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoHotel(IdentificadorViagem, predicate);
            }
        }

        public List<GastoRefeicao> ListarGastoRefeicao(int? IdentificadorViagem, Expression<Func<GastoRefeicao, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoRefeicao(IdentificadorViagem, predicate);
            }
        }
        public List<GastoViagemAerea> ListarGastoViagemAerea(int? IdentificadorViagem, Expression<Func<GastoViagemAerea, bool>> predicate)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGastoViagemAerea(IdentificadorViagem, predicate);
            }
        }

        public IList<Usuario> CarregarParticipantesViagem(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarParticipantesViagem(IdentificadorViagem);
            }
        }
        public IList<Usuario> CarregarParticipantesViagem(int? IdentificadorViagem, int? IdentificadorUsuario)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.CarregarParticipantesViagem(IdentificadorViagem, IdentificadorUsuario);
            }
        }


        public List<Atracao> ListarAtracao(int IdentificadorViagem, DateTime? DataChegadaDe, DateTime? DataChegadaAte,
             DateTime? DataPartidaDe, DateTime? DataPartidaAte, string Nome, string Tipo, int Situacao, int? IdentificadorCidade, int? IdentificadorAtracao)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                var ListaAtracoes = repositorio.ListarAtracao(IdentificadorViagem, DataChegadaDe, DataChegadaAte, DataPartidaDe, DataPartidaAte, Nome, Tipo, Situacao,
                    IdentificadorCidade, IdentificadorAtracao);
                foreach (var itemAtracao in ListaAtracoes)
                {
                    if (itemAtracao.ItemAtracaoPai != null)
                        itemAtracao.ItemAtracaoPai.Atracoes = null;
                }
                return ListaAtracoes;
            }
        }

        public List<Gasto> ListarGasto(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Descricao,
           int? IdentificadorUsuario, int? IdentificadorGasto)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarGasto(IdentificadorViagem, DataDe, DataAte, Descricao, IdentificadorUsuario, IdentificadorGasto);

            }
        }

        public List<Refeicao> ListarRefeicao(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte,
             string Nome, string Tipo, int? IdentificadorCidade, int? IdentificadorRefeicao)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarRefeicao(IdentificadorViagem, DataDe, DataAte, Nome, Tipo, IdentificadorCidade, IdentificadorRefeicao);

            }
        }

        public List<Hotel> ListarHotel(int IdentificadorViagem, DateTime? DataCheckInDe, DateTime? DataCheckInAte,
         DateTime? DataCheckOutDe, DateTime? DataCheckOutAte, string Nome, int Situacao, int? IdentificadorCidade, int? IdentificadorHotel)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarHotel(IdentificadorViagem, DataCheckInDe, DataCheckInAte, DataCheckOutDe, DataCheckOutAte, Nome, Situacao, IdentificadorCidade, IdentificadorHotel);

            }
        }

        public List<ViagemAerea> ListarViagemAerea(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Companhia, int? Tipo,
           int Situacao, int? IdentificadorCidadeOrigem, int? IdentificadorCidadeDestino, int? IdentificadorViagemAerea)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarViagemAerea(IdentificadorViagem, DataDe, DataAte, Companhia, Tipo, Situacao, IdentificadorCidadeOrigem, IdentificadorCidadeDestino, IdentificadorViagemAerea);
            }
        }

        public List<Carro> ListarCarro(int IdentificadorViagem, string Locadora, string Descricao, string Modelo,
     DateTime? DataRetiradaDe, DateTime? DataRetiradaAte, DateTime? DataDevolucaoDe, DateTime? DataDevolucaoAte, int? IdentificadorCarro)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCarro(IdentificadorViagem, Locadora, Descricao, Modelo, DataRetiradaDe, DataRetiradaAte, DataDevolucaoDe, DataDevolucaoAte, IdentificadorCarro);
            }

        }

        public List<Loja> ListarLoja(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte,
           string Nome, int? IdentificadorCidade, int? IdentificadorLoja)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarLoja(IdentificadorViagem, DataDe, DataAte, Nome, IdentificadorCidade, IdentificadorLoja);
            }
        }

        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, int? IdentificadorViagem, List<int?> Status, int? IdentificadorListaCompra)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarListaCompra(IdentificadorViagem, IdentificadorViagem, Status, IdentificadorListaCompra);
            }
        }

        public List<Comentario> ListarComentario(int? IdentificadorUsuario, int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, int? IdentificadorCidade, int? IdentificadorComentario)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarComentario(IdentificadorUsuario, IdentificadorViagem, DataDe, DataAte, IdentificadorCidade, IdentificadorComentario);
            }
        }

        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, int? IdentificadorViagem, int? Status, string Destinatario, int? IdentificadorUsuarioPedido,
           string Marca, string Descricao)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarListaCompra(IdentificadorUsuario, IdentificadorViagem, Status, Destinatario, IdentificadorUsuarioPedido, Marca, Descricao);
            }
        }

        public List<AporteDinheiro> ListarAporteDinheiro(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime? DataDe, DateTime? DataAte)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarAporteDinheiro(IdentificadorUsuario, IdentificadorViagem, Moeda, DataDe, DataAte);
            }
        }

        public List<Sugestao> ListarSugestao(int? IdentificadorUsuario, int? IdentificadorViagem, string Nome, string Tipo, int? IdentificadorCidade, int? Situacao)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarSugestao(IdentificadorUsuario, IdentificadorViagem, Nome, Tipo,IdentificadorCidade,Situacao);
            }
        }

        public List<Cidade> ListarCidadePai(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCidadePai(IdentificadorViagem);
            }
        }

        public List<Cidade> ListarCidadeNaoAssociadasFilho(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCidadeNaoAssociadasFilho(IdentificadorViagem);
            }
        }

        public List<Cidade> ListarCidadeNaoAssociadasPai(int? IdentificadorViagem, int? IdentificadorCidadePai)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCidadeNaoAssociadasPai(IdentificadorViagem, IdentificadorCidadePai);
            }
        }

        public List<ManutencaoCidadeGrupo> ListarManutencaoCidadeGrupo(int? IdentificadorViagem, int? IdentificadorCidadePai)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarManutencaoCidadeGrupo(IdentificadorViagem, IdentificadorCidadePai);
            }
        }

        public List<CidadeGrupo> ListarCidadeGrupo(int? IdentificadorViagem, int? IdentificadorCidadePai)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCidadeGrupo(IdentificadorViagem, IdentificadorCidadePai);
            }

        }

        public List<CalendarioPrevisto> ListarCalendarioPrevisto(int? IdentificadorViagem)
        {
            using (ViagemRepository repositorio = new ViagemRepository())
            {
                return repositorio.ListarCalendarioPrevisto(IdentificadorViagem);
            }
        }

        public void SalvarAtracaoSimples(Atracao itemGravar)
        {
           
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarAtracaoSimples(itemGravar);
                    
                }
            
        }

        public void SalvarReabastecimentoCompleto(Reabastecimento itemGravar)
        {
            LimparValidacao();
            ValidateService(itemGravar);
            ValidarRegrasNegocioReabastecimento(itemGravar);
            if (IsValid())
            {
                using (ViagemRepository data = new ViagemRepository())
                {
                    data.SalvarReabastecimentoCompleto(itemGravar);
                    Message msg = new Message();
                    msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens("Viagem_SalvarReabastecimento_OK") });
                    ServiceResult resultado = new ServiceResult();
                    resultado.Success = true;
                    resultado.Messages.Add(msg);
                    serviceResult.Add(resultado);
                }
            }
        }

        public List<DeParaIdentificador> SincronizarDados(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem)
        {
            List<DeParaIdentificador> listaResultado = new List<DeParaIdentificador>();
            SincronizarCotacoes(itemDados, identificadorViagem, listaResultado);
            SincronizarListaCompra(itemDados, identificadorViagem, listaResultado);
            SincronizarCalendariosPrevistos(itemDados, identificadorViagem, listaResultado);
            SincronizarComentarios(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarSugestoes(itemDados, identificadorViagem, listaResultado);
            SincronizarAporteDinheiro(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarAtracoes(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarRefeicoes(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarLojas(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarHoteis(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarCarros(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarViagemAereas(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarCarroDeslocamento(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarReabastecimentos(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarHotelEvento(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoCompra(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarItemCompra(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGasto(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoAtracao(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoHotel(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoRefeicao(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoViagemAerea(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarGastoCarro(itemDados, identificadorUsuario, identificadorViagem, listaResultado);
            SincronizarPosicoes(itemDados, identificadorUsuario, identificadorViagem);
            return listaResultado;
        }
        public void SalvarPosicaoLista(List<Posicao> Lista)
        {
            using (ViagemRepository data = new ViagemRepository())
            {
                data.SalvarPosicaoLista(Lista);
            }
        }
            
        private void SincronizarPosicoes(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem)
        {
            foreach (var item in itemDados.Posicoes)
            {
                item.IdentificadorUsuario = identificadorUsuario;
                item.IdentificadorViagem = identificadorViagem;
                item.Identificador = null;
            }
            SalvarPosicaoLista(itemDados.Posicoes);
        }

        private void SincronizarGastoCarro(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.GastosCarro)
            {
                var itemBase = SelecionarAluguelGasto(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }
                if (item.IdentificadorGasto.HasValue && item.IdentificadorGasto < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGasto = ItemNovoCodigo.IdentificadorDetino;

                }
                if (item.IdentificadorCarro.HasValue && item.IdentificadorCarro < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorCarro = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GC";
                if (item.Identificador < 0)
                    item.Identificador = null;



                SalvarAluguelGasto(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }


        private void SincronizarGastoHotel(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.GastosHotel)
            {
                var itemBase = SelecionarGastoHotel(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }
                if (item.IdentificadorGasto.HasValue && item.IdentificadorGasto < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGasto = ItemNovoCodigo.IdentificadorDetino;

                }
                if (item.IdentificadorHotel.HasValue && item.IdentificadorHotel < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorHotel && d.TipoObjeto == "H").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorHotel = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GH";
                if (item.Identificador < 0)
                    item.Identificador = null;



                SalvarGastoHotel(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }


        private void SincronizarGastoViagemAerea(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.GastosDeslocamento)
            {
                var itemBase = SelecionarGastoViagemAerea(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }
                if (item.IdentificadorGasto.HasValue && item.IdentificadorGasto < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGasto = ItemNovoCodigo.IdentificadorDetino;

                }
                if (item.IdentificadorViagemAerea.HasValue && item.IdentificadorViagemAerea < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorViagemAerea && d.TipoObjeto == "VA").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorViagemAerea = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GV";
                if (item.Identificador < 0)
                    item.Identificador = null;



                SalvarGastoViagemAerea(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }


        private void SincronizarGastoRefeicao(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.GastosRefeicao)
            {
                var itemBase = SelecionarGastoRefeicao(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }
                if (item.IdentificadorGasto.HasValue && item.IdentificadorGasto < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGasto = ItemNovoCodigo.IdentificadorDetino;

                }
                if (item.IdentificadorRefeicao.HasValue && item.IdentificadorRefeicao < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorRefeicao && d.TipoObjeto == "R").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorRefeicao = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GR";
                if (item.Identificador < 0)
                    item.Identificador = null;



                SalvarGastoRefeicao(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }


        private void SincronizarGastoAtracao(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.GastosAtracao)
            {
                var itemBase = SelecionarGastoAtracao(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }
                if (item.IdentificadorGasto.HasValue && item.IdentificadorGasto < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGasto = ItemNovoCodigo.IdentificadorDetino;

                }
                if (item.IdentificadorAtracao.HasValue && item.IdentificadorAtracao < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorAtracao = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GA";
                if (item.Identificador < 0)
                    item.Identificador = null;
    


                SalvarGastoAtracao(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }

        private void SincronizarGasto(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemGasto in itemDados.Gastos)
            {
                var itemBase = SelecionarGasto(itemGasto.Identificador);
                if (itemBase != null && itemBase.DataAtualizacao > itemGasto.DataAtualizacao || itemGasto.DataExclusao.HasValue)
                    continue;
                itemGasto.Reabastecimentos = new List<ReabastecimentoGasto>();
                itemGasto.Compras = new List<GastoCompra>();
                if (itemGasto.DataExclusao.HasValue)
                {
                    itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var item in itemGasto.Alugueis.Where(d => !d.DataExclusao.HasValue))
                        item.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var item in itemGasto.Atracoes.Where(d => !d.DataExclusao.HasValue))
                        item.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var item in itemGasto.Hoteis.Where(d => !d.DataExclusao.HasValue))
                        item.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var item in itemGasto.Refeicoes.Where(d => !d.DataExclusao.HasValue))
                        item.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var item in itemGasto.ViagenAereas.Where(d => !d.DataExclusao.HasValue))
                        item.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemGasto.Usuarios = new List<GastoDividido>();
                }


               
                itemGasto.Compras = ListarGastoCompra(d => d.IdentificadorGasto == itemGasto.Identificador);
                itemGasto.Reabastecimentos = ListarReabastecimentoGastos(d => d.IdentificadorGasto == itemGasto.Identificador);

                itemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemGasto.Identificador;
                itemDePara.TipoObjeto = "G";
                if (itemGasto.Identificador < 0)
                    itemGasto.Identificador = null;

                foreach (var item in itemGasto.Alugueis.Where(d => !d.DataExclusao.HasValue))
                {
                    item.IdentificadorGasto = itemGasto.Identificador;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    if (item.IdentificadorCarro.HasValue && item.IdentificadorCarro < 0)
                    {
                        var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                        if (ItemNovoCodigo != null)
                            item.IdentificadorCarro = ItemNovoCodigo.IdentificadorDetino;

                    }
                }
                foreach (var item in itemGasto.Atracoes.Where(d => !d.DataExclusao.HasValue))
                {
                    item.IdentificadorGasto = itemGasto.Identificador;

                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    if (item.IdentificadorAtracao.HasValue && item.IdentificadorAtracao < 0)
                    {
                        var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                        if (ItemNovoCodigo != null)
                            item.IdentificadorAtracao = ItemNovoCodigo.IdentificadorDetino;

                    }
                }
                foreach (var item in itemGasto.Hoteis.Where(d => !d.DataExclusao.HasValue))
                {
                    {
                        item.IdentificadorGasto = itemGasto.Identificador;

                        item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                        if (item.IdentificadorHotel.HasValue && item.IdentificadorHotel < 0)
                        {
                            var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorHotel && d.TipoObjeto == "H").FirstOrDefault();
                            if (ItemNovoCodigo != null)
                                item.IdentificadorHotel = ItemNovoCodigo.IdentificadorDetino;

                        }
                    }
                }
                foreach (var item in itemGasto.Refeicoes.Where(d => !d.DataExclusao.HasValue))
                {
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;

                    if (item.IdentificadorRefeicao.HasValue && item.IdentificadorRefeicao < 0)
                    {
                        var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorRefeicao && d.TipoObjeto == "R").FirstOrDefault();
                        if (ItemNovoCodigo != null)
                            item.IdentificadorRefeicao = ItemNovoCodigo.IdentificadorDetino;

                    }
                }
                foreach (var item in itemGasto.ViagenAereas.Where(d => !d.DataExclusao.HasValue))
                {
                    item.IdentificadorGasto = itemGasto.Identificador;

                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    if (item.IdentificadorViagemAerea.HasValue && item.IdentificadorViagemAerea < 0)
                    {
                        var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorViagemAerea && d.TipoObjeto == "VA").FirstOrDefault();
                        if (ItemNovoCodigo != null)
                            item.IdentificadorViagemAerea = ItemNovoCodigo.IdentificadorDetino;

                    }
                }
                foreach (var item in itemGasto.Usuarios)
                    item.IdentificadorGasto = itemGasto.Identificador;
                itemGasto.IdentificadorViagem = identificadorViagem;
                itemGasto.IdentificadorCidade = RetornarCidadeGeocoding(itemGasto.Latitude, itemGasto.Longitude);
                SalvarGasto_Completo(itemGasto);

                itemDePara.IdentificadorDetino = itemGasto.Identificador;
                listaResultado.Add(itemDePara);

                var itemGastoBase = SelecionarGasto_Completo(itemGasto.Identificador);
                foreach (var item in itemGastoBase.Alugueis.Where(d=>!d.DataExclusao.HasValue))
                {
                    var itemBaseTipo = itemGastoBase.Alugueis.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorCarro == item.IdentificadorCarro).FirstOrDefault();
                    if (itemBaseTipo != null)
                        listaResultado.Add(new DeParaIdentificador() { IdentificadorDetino = itemBaseTipo.Identificador, IdentificadorOrigem = item.Identificador, TipoObjeto = "GC" });
                }
                foreach (var item in itemGastoBase.Atracoes.Where(d => !d.DataExclusao.HasValue))
                {
                    var itemBaseTipo = itemGastoBase.Atracoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorAtracao == item.IdentificadorAtracao).FirstOrDefault();
                    if (itemBaseTipo != null)
                        listaResultado.Add(new DeParaIdentificador() { IdentificadorDetino = itemBaseTipo.Identificador, IdentificadorOrigem = item.Identificador, TipoObjeto = "GA" });
                }
                foreach (var item in itemGastoBase.Hoteis.Where(d => !d.DataExclusao.HasValue))
                {
                    var itemBaseTipo = itemGastoBase.Hoteis.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorHotel == item.IdentificadorHotel).FirstOrDefault();
                    if (itemBaseTipo != null)
                        listaResultado.Add(new DeParaIdentificador() { IdentificadorDetino = itemBaseTipo.Identificador, IdentificadorOrigem = item.Identificador, TipoObjeto = "GH" });
                }
                foreach (var item in itemGastoBase.ViagenAereas.Where(d => !d.DataExclusao.HasValue))
                {
                    var itemBaseTipo = itemGastoBase.ViagenAereas.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagemAerea == item.IdentificadorViagemAerea).FirstOrDefault();
                    if (itemBaseTipo != null)
                        listaResultado.Add(new DeParaIdentificador() { IdentificadorDetino = itemBaseTipo.Identificador, IdentificadorOrigem = item.Identificador, TipoObjeto = "GV" });
                }

                foreach (var item in itemGastoBase.Refeicoes.Where(d => !d.DataExclusao.HasValue))
                {
                    var itemBaseTipo = itemGastoBase.Refeicoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorRefeicao == item.IdentificadorRefeicao).FirstOrDefault();
                    if (itemBaseTipo != null)
                        listaResultado.Add(new DeParaIdentificador() { IdentificadorDetino = itemBaseTipo.Identificador, IdentificadorOrigem = item.Identificador, TipoObjeto = "GR" });
                }

            }

        }


        private void SincronizarItemCompra(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.ItensComprados)
            {
                var itemBase = SelecionarItemCompra(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }

                if (item.IdentificadorGastoCompra.HasValue && item.IdentificadorGastoCompra < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorGastoCompra && d.TipoObjeto == "GL").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorGastoCompra = ItemNovoCodigo.IdentificadorDetino;

                }

                if (item.IdentificadorListaCompra.HasValue && item.IdentificadorListaCompra < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorListaCompra && d.TipoObjeto == "LC").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorListaCompra = ItemNovoCodigo.IdentificadorDetino;
                    else
                        item.IdentificadorListaCompra = null;
                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "IC";
                if (item.Identificador < 0)
                    item.Identificador = null;              


                SalvarItemCompra(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

             

            }

        }

        private void SincronizarGastoCompra(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Compras)
            {
                var itemBase = SelecionarGastoCompra(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.ItemGasto.Usuarios = new List<GastoDividido>();
                }

                if (item.IdentificadorLoja.HasValue && item.IdentificadorLoja < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorLoja && d.TipoObjeto == "L").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorLoja = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                item.ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "GL";
                if (item.Identificador < 0)
                    item.Identificador = null;
                DeParaIdentificador itemDeParaGasto = new DeParaIdentificador();
                itemDeParaGasto.IdentificadorOrigem = item.Identificador;
                itemDeParaGasto.TipoObjeto = "G";
                if (item.ItemGasto.Identificador < 0)
                    item.ItemGasto.Identificador = null;



                SalvarGastoCompra_Completo(item);

                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

                GastoCompra itemAtualizado = SelecionarGastoCompra(item.Identificador);
                itemDeParaGasto.IdentificadorDetino = itemAtualizado.IdentificadorGasto;
                listaResultado.Add(itemDeParaGasto);

            }

        }

        private void SincronizarHotelEvento(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.EventosHotel)
            {
                var itemBase = SelecionarHotelEvento(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                }

                if (item.IdentificadorHotel.HasValue && item.IdentificadorHotel < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorHotel && d.TipoObjeto == "H").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorHotel = ItemNovoCodigo.IdentificadorDetino;

                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "HE";
                if (item.Identificador < 0)
                    item.Identificador = null;

               

                SalvarHotelEvento(item);
              
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);

            }

        }

        private void SincronizarReabastecimentos(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Reabastecimento)
            {
                var itemBase = SelecionarReabastecimento(item.Identificador);
                if (itemBase != null && itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Gastos[0].ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Gastos[0].DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Gastos[0].ItemGasto.Usuarios = new List<GastoDividido>();
                }
                item.IdentificadorCidade = RetornarCidadeGeocoding(item.Latitude, item.Longitude);

                if (item.IdentificadorCarro.HasValue && item.IdentificadorCarro < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorCarro = ItemNovoCodigo.IdentificadorDetino;

                }
                item.Gastos[0].ItemGasto.IdentificadorCidade = item.IdentificadorCidade;
                item.Gastos[0].ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                item.Gastos[0].ItemGasto.IdentificadorViagem = identificadorViagem;

                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "CR";
                if (item.Identificador < 0)
                    item.Identificador = null;
                item.Gastos[0].IdentificadorReabastecimento = item.Identificador;
                DeParaIdentificador itemDeParaGasto = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Gastos[0].IdentificadorGasto;
                itemDePara.TipoObjeto = "G";

                DeParaIdentificador itemDeParaGastoReabastecimento = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Gastos[0].IdentificadorGasto;
                itemDePara.TipoObjeto = "GR";


                SalvarReabastecimentoCompleto(item);
                var itemReabastecimento = SelecionarReabastecimento(item.Identificador);
               
                foreach (var itemAvaliacao in item.Gastos[0].ItemGasto.Usuarios)
                {
                    var itemgravado = itemReabastecimento.Gastos[0].ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "GD" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDeParaGasto.IdentificadorDetino = itemReabastecimento.Gastos[0].IdentificadorGasto;
                itemDeParaGastoReabastecimento.IdentificadorDetino = itemReabastecimento.Gastos[0].Identificador;
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
                listaResultado.Add(itemDeParaGasto);
                listaResultado.Add(itemDeParaGastoReabastecimento);
            }

        }
        private void SincronizarCarroDeslocamento(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.CarroDeslocamentos)
            {
                var itemBase = SelecionarCarroDeslocamento(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Usuarios = new List<CarroDeslocamentoUsuario>();
                }
                if (item.IdentificadorCarro.HasValue && item.IdentificadorCarro < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorCarro = ItemNovoCodigo.IdentificadorDetino;
                   
                }
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "CD";
                if (item.Identificador < 0)
                    item.Identificador = null;

                foreach (var itemAvaliacao in item.Usuarios)
                    itemAvaliacao.IdentificadorCarroDeslocamento = item.Identificador;

                DeParaIdentificador itemDeParaPartida = null;

                if (item.ItemCarroEventoPartida != null)
                {
                    itemDeParaPartida = new DeParaIdentificador() { IdentificadorOrigem = item.IdentificadorCarroEventoPartida, TipoObjeto = "CE" };
                    item.ItemCarroEventoPartida.IdentificadorCidade = RetornarCidadeGeocoding(item.ItemCarroEventoPartida.Latitude, item.ItemCarroEventoPartida.Longitude);
                    item.ItemCarroEventoPartida.DataAtualizacao = DateTime.Now.ToUniversalTime();

                }
                item.IdentificadorCarroEventoPartida = null;
                DeParaIdentificador itemDeParaChegada = null;

                if (item.ItemCarroEventoChegada != null)
                {
                    itemDeParaChegada = new DeParaIdentificador() { IdentificadorOrigem = item.IdentificadorCarroEventoChegada, TipoObjeto = "CE" };
                    if (item.ItemCarroEventoChegada.Data == new DateTime(1900, 01, 01))
                        item.ItemCarroEventoChegada.Data = null;
                    item.ItemCarroEventoChegada.IdentificadorCidade = RetornarCidadeGeocoding(item.ItemCarroEventoChegada.Latitude, item.ItemCarroEventoChegada.Longitude);
                    item.ItemCarroEventoChegada.DataAtualizacao = DateTime.Now.ToUniversalTime();

                }
                item.IdentificadorCarroEventoChegada = null;

                SalvarCarroDeslocamento_Evento(item);
                var itemCarroDeslocamneto = SelecionarCarroDeslocamento(item.Identificador);
                if (itemDeParaPartida != null)
                {
                    itemDeParaPartida.IdentificadorDetino = itemCarroDeslocamneto.IdentificadorCarroEventoPartida;
                    listaResultado.Add(itemDeParaPartida);
                }
                if (itemDeParaChegada != null)
                {
                    itemDeParaChegada.IdentificadorDetino = itemCarroDeslocamneto.IdentificadorCarroEventoChegada;
                    listaResultado.Add(itemDeParaChegada);
                }
                foreach (var itemAvaliacao in item.Usuarios)
                {
                    var itemgravado = itemCarroDeslocamneto.Usuarios.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "CDU" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }

        }
        private void SincronizarViagemAereas(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Deslocamentos)
            {
                var itemBase = SelecionarViagemAerea(item.Identificador);
                if (itemBase != null && itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                  if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Avaliacoes = new List<AvaliacaoAerea>();
                    item.Aeroportos = new List<ViagemAereaAeroporto>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "VA";
                if (item.Identificador < 0)
                    item.Identificador = null;

                foreach (var itemAvaliacao in item.Avaliacoes)
                    itemAvaliacao.IdentificadorViagemAerea = item.Identificador;

                Dictionary<ViagemAereaAeroporto, DeParaIdentificador> ListaSincroniza = new Dictionary<ViagemAereaAeroporto, DeParaIdentificador>();
                foreach (var itemAvaliacao in item.Aeroportos)
                {
                    itemAvaliacao.IdentificadorViagemAerea = item.Identificador;
                    itemAvaliacao.IdentificadorCidade = RetornarCidadeGeocoding(itemAvaliacao.Latitude, itemAvaliacao.Longitude);
                    DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, TipoObjeto = "VAA" };
                    listaResultado.Add(itemDP);
                    ListaSincroniza.Add(itemAvaliacao, itemDP);
                }


                SalvarViagemAerea(item);
                var itemViagemAereaCompleta = SelecionarViagemAerea_Completa(item.Identificador);
                foreach (var itemAvaliacao in item.Avaliacoes)
                {
                    var itemgravado = itemViagemAereaCompleta.Avaliacoes.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AVA" };
                        listaResultado.Add(itemDP);
                    }
                }
                foreach (var itemAvaliacao in item.Aeroportos)
                {
                    var itemgravado = itemViagemAereaCompleta.Aeroportos.Where(d => d.Identificador == itemAvaliacao.Identificador).FirstOrDefault();
                    itemgravado.ItemViagemAerea = null;
                    DeParaIdentificador itemDP = ListaSincroniza[itemAvaliacao];
                    itemDP.IdentificadorDetino = itemAvaliacao.Identificador;
                    itemDP.ItemRetorno = itemgravado;

                }
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }

        }


        private void SincronizarCarros(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Carros)
            {
                var itemBase = SelecionarCarro(item.Identificador);
                if (itemBase != null && itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
             
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Avaliacoes = new List<AvaliacaoAluguel>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "C";
                if (item.Identificador < 0)
                    item.Identificador = null;
                foreach (var itemAvaliacao in item.Avaliacoes)
                    itemAvaliacao.IdentificadorCarro = item.Identificador;
                DeParaIdentificador itemDeParaDevolucao = null;

                if (item.ItemCarroEventoDevolucao != null)
                {
                    itemDeParaDevolucao = new DeParaIdentificador() { IdentificadorOrigem = item.IdentificadorCarroEventoDevolucao, TipoObjeto = "CE" };
                    item.ItemCarroEventoDevolucao.IdentificadorCidade = RetornarCidadeGeocoding(item.ItemCarroEventoDevolucao.Latitude, item.ItemCarroEventoDevolucao.Longitude);
                    item.ItemCarroEventoDevolucao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    
                }
                item.IdentificadorCarroEventoDevolucao = null;
                DeParaIdentificador itemDeParaRetirada = null;

                if (item.ItemCarroEventoRetirada != null)
                {
                    itemDeParaRetirada = new DeParaIdentificador() { IdentificadorOrigem = item.IdentificadorCarroEventoRetirada, TipoObjeto = "CE" };

                    item.ItemCarroEventoRetirada.IdentificadorCidade = RetornarCidadeGeocoding(item.ItemCarroEventoRetirada.Latitude, item.ItemCarroEventoRetirada.Longitude);
                    item.ItemCarroEventoRetirada.DataAtualizacao = DateTime.Now.ToUniversalTime();

                }
                item.IdentificadorCarroEventoRetirada = null;

                SalvarCarro_Evento(item);
                var itemCarroCompleta = SelecionarCarro_Completo(item.Identificador);
                if (itemDeParaDevolucao != null )
                {
                    itemDeParaDevolucao.IdentificadorDetino = itemCarroCompleta.IdentificadorCarroEventoDevolucao;
                    listaResultado.Add(itemDeParaDevolucao);
                }
                if (itemDeParaRetirada != null)
                {
                    itemDeParaRetirada.IdentificadorDetino = itemCarroCompleta.IdentificadorCarroEventoRetirada;
                    listaResultado.Add(itemDeParaRetirada);
                }
                foreach (var itemAvaliacao in item.Avaliacoes)
                {
                    var itemgravado = itemCarroCompleta.Avaliacoes.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AC" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }

        }

        private void SincronizarHoteis(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Hoteis)
            {
                var itemBase = SelecionarHotel(item.Identificador);
                if (itemBase != null && itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Avaliacoes = new List<HotelAvaliacao>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                item.Eventos = ListarHotelEvento(d => d.IdentificadorHotel == item.Identificador);
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "H";
                if (item.Identificador < 0)
                    item.Identificador = null;
                item.IdentificadorCidade = RetornarCidadeGeocoding(item.Latitude, item.Longitude);

                foreach (var itemAvaliacao in item.Avaliacoes)
                    itemAvaliacao.IdentificadorHotel = item.Identificador;

                SalvarHotel(item);
                var itemHotelCompleta = SelecionarHotel_Completo(item.Identificador);
                foreach (var itemAvaliacao in item.Avaliacoes)
                {
                    var itemgravado = itemHotelCompleta.Avaliacoes.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AH" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDePara.ItemRetorno = SelecionarHotel(item.Identificador);
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }

        }

        private void SincronizarLojas(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Lojas)
            {
                var itemBase = SelecionarLoja(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;

                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Avaliacoes = new List<AvaliacaoLoja>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "L";
                if (item.Identificador < 0)
                    item.Identificador = null;
                foreach (var itemAvaliacao in item.Avaliacoes)
                    itemAvaliacao.IdentificadorLoja = item.Identificador;

                item.IdentificadorCidade = RetornarCidadeGeocoding(item.Latitude, item.Longitude);
                if (item.IdentificadorAtracao.HasValue && item.IdentificadorAtracao < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorAtracao = ItemNovoCodigo.IdentificadorDetino;
                    else
                    {
                        item.IdentificadorAtracao = null;
                    }
                }

                SalvarLoja(item);
                var itemLojaCompleta = SelecionarLoja_Completo(item.Identificador);
                foreach (var itemAvaliacao in item.Avaliacoes)
                {
                    var itemgravado = itemLojaCompleta.Avaliacoes.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AL" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDePara.ItemRetorno = SelecionarLoja(item.Identificador);
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }

        }

        private void SincronizarRefeicoes(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var item in itemDados.Refeicoes)
            {
                var itemBase = SelecionarRefeicao(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;
  
                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Pedidos = new List<RefeicaoPedido>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "R";
                if (item.Identificador < 0)
                    item.Identificador = null;

                foreach (var itemAvaliacao in item.Pedidos)
                    itemAvaliacao.IdentificadorRefeicao = item.Identificador;


                item.IdentificadorCidade = RetornarCidadeGeocoding(item.Latitude, item.Longitude);
                if (item.IdentificadorAtracao.HasValue && item.IdentificadorAtracao < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorAtracao = ItemNovoCodigo.IdentificadorDetino;
                    else
                    {
                        item.IdentificadorAtracao = null;
                    }
                }

                SalvarRefeicao(item);
                var itemRefeicaoCompleta = SelecionarRefeicao_Completa(item.Identificador);
                foreach (var itemAvaliacao in item.Pedidos)
                {
                    var itemgravado = itemRefeicaoCompleta.Pedidos.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AR" };
                        listaResultado.Add(itemDP);
                    }
                }
                itemDePara.ItemRetorno = SelecionarRefeicao(item.Identificador);
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }
           
        }

        private void SincronizarAtracoes(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            Dictionary<int?, int?> AjustesAtracaoPai = new Dictionary<int?, int?>();
            foreach (var item in itemDados.Atracoes)
            {
                var itemBase = SelecionarAtracao(item.Identificador);
                if (itemBase != null &&  itemBase.DataAtualizacao > item.DataAtualizacao || item.DataExclusao.HasValue)
                    continue;

                if (item.DataExclusao.HasValue)
                {
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    item.Avaliacoes = new List<AvaliacaoAtracao>();
                }
                item.IdentificadorViagem = identificadorViagem;
                item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = item.Identificador;
                itemDePara.TipoObjeto = "A";
                if (item.Identificador < 0)
                    item.Identificador = null;
                foreach (var itemAvaliacao in item.Avaliacoes)
                    itemAvaliacao.IdentificadorAtracao = item.Identificador;

                item.IdentificadorCidade = RetornarCidadeGeocoding(item.Latitude, item.Longitude);
                int? IdentificadorPaiAjustar = null;
                if (item.IdentificadorAtracaoPai.HasValue && item.IdentificadorAtracaoPai < 0)
                {
                    var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracaoPai && d.TipoObjeto == "A").FirstOrDefault();
                    if (ItemNovoCodigo != null)
                        item.IdentificadorAtracaoPai = ItemNovoCodigo.IdentificadorDetino;
                    else
                    {
                        IdentificadorPaiAjustar = item.IdentificadorAtracaoPai;
                        item.IdentificadorAtracaoPai = null;
                    }
                }                

                SalvarAtracao(item);
                var itemAtracaoCompleta = SelecionarAtracao_Completo(item.Identificador);
                foreach (var itemAvaliacao in item.Avaliacoes)
                {
                    var itemgravado = itemAtracaoCompleta.Avaliacoes.Where(d => d.IdentificadorUsuario == itemAvaliacao.IdentificadorUsuario).FirstOrDefault();
                    if (itemgravado != null)
                    {
                        DeParaIdentificador itemDP = new DeParaIdentificador() { IdentificadorOrigem = itemAvaliacao.Identificador, IdentificadorDetino = itemgravado.Identificador, TipoObjeto = "AA" };
                        listaResultado.Add(itemDP);
                    }
                }
                if (IdentificadorPaiAjustar.HasValue)
                    AjustesAtracaoPai.Add(item.Identificador, IdentificadorPaiAjustar);
                itemDePara.ItemRetorno = SelecionarAtracao(item.Identificador);
                itemDePara.IdentificadorDetino = item.Identificador;
                listaResultado.Add(itemDePara);
            }
            foreach (var itemAjuste in AjustesAtracaoPai)
            {
                var ItemNovoCodigo = listaResultado.Where(d => d.IdentificadorOrigem == itemAjuste.Value && d.TipoObjeto == "A").FirstOrDefault();
                if (ItemNovoCodigo != null)
                {
                    var itemBase = SelecionarAtracao(itemAjuste.Key);
                    itemBase.IdentificadorAtracaoPai = ItemNovoCodigo.IdentificadorDetino;
                    SalvarAtracaoSimples(itemBase);
                }

            }
        }

        private void SincronizarAporteDinheiro(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemAporteDinheiro in itemDados.AportesDinheiro)
            {
                var itemAporteBase = SelecionarAporteDinheiro(itemAporteDinheiro.Identificador);
                if (itemAporteBase != null &&  itemAporteBase.DataAtualizacao > itemAporteDinheiro.DataAtualizacao || itemAporteDinheiro.DataExclusao.HasValue)
                    continue;

                if (itemAporteDinheiro.DataExclusao.HasValue)
                    itemAporteDinheiro.DataExclusao = DateTime.Now.ToUniversalTime();
                itemAporteDinheiro.IdentificadorViagem = identificadorViagem;
                itemAporteDinheiro.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAporteDinheiro.IdentificadorUsuario = identificadorUsuario;
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemAporteDinheiro.Identificador;
                itemDePara.TipoObjeto = "AD";
                if (itemAporteDinheiro.Identificador < 0)
                    itemAporteDinheiro.Identificador = null;
                DeParaIdentificador itemDeParaGasto = null;

                if (itemAporteDinheiro.ItemGasto != null)
                {
                    itemDeParaGasto = new DeParaIdentificador();
                    itemDeParaGasto.IdentificadorOrigem = itemAporteDinheiro.ItemGasto.Identificador;
                    if (itemAporteDinheiro.DataExclusao.HasValue)
                        itemAporteDinheiro.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemAporteDinheiro.ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAporteDinheiro.ItemGasto.IdentificadorUsuario = identificadorUsuario;
                    itemAporteDinheiro.ItemGasto.IdentificadorViagem = identificadorViagem;
                    itemAporteDinheiro.ItemGasto.Data = itemAporteDinheiro.DataAporte;
                    if (itemAporteDinheiro.Identificador < 0)
                    {
                        itemAporteDinheiro.ItemGasto.Identificador = null;
                        itemAporteDinheiro.IdentificadorGasto = null;
                    }
                    itemDeParaGasto.TipoObjeto = "G";
                }
                
                SalvarAporteDinheiroCompleto(itemAporteDinheiro);
                var itemAporteDinheiroNovo = SelecionarAporteDinheiro(itemAporteDinheiro.Identificador);
                itemDePara.IdentificadorDetino = itemAporteDinheiroNovo.Identificador;
                listaResultado.Add(itemDePara);
                if (itemDeParaGasto != null)
                {
                    itemDeParaGasto.IdentificadorDetino = itemAporteDinheiroNovo.ItemGasto.Identificador;
                    listaResultado.Add(itemDeParaGasto);
                }
            }
        }

        private void SincronizarCotacoes(ClasseSincronizacao itemDados, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemCotacao in itemDados.CotacoesMoeda)
            {
                var itemAporteBase = SelecionarCotacaoMoeda(itemCotacao.Identificador);
                if (itemAporteBase != null && itemAporteBase.DataAtualizacao > itemCotacao.DataAtualizacao || itemCotacao.DataExclusao.HasValue)
                    continue;
                if (itemCotacao.DataExclusao.HasValue)
                    itemCotacao.DataExclusao = DateTime.Now.ToUniversalTime();
                itemCotacao.IdentificadorViagem = identificadorViagem;
                itemCotacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemCotacao.Identificador;
                itemDePara.TipoObjeto = "CM";
                if (itemCotacao.Identificador < 0)
                    itemCotacao.Identificador = null;
                SalvarCotacaoMoeda(itemCotacao);
                itemDePara.IdentificadorDetino = itemCotacao.Identificador;
                listaResultado.Add(itemDePara);
            }
        }

        private void SincronizarListaCompra(ClasseSincronizacao itemDados, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemLC in itemDados.ListaCompra)
            {
                var itemAporteBase = SelecionarListaCompra(itemLC.Identificador);
                if (itemAporteBase != null && itemAporteBase.DataAtualizacao > itemLC.DataAtualizacao || itemLC.DataExclusao.HasValue)
                    continue;
                if (itemLC.DataExclusao.HasValue)
                    itemLC.DataExclusao = DateTime.Now.ToUniversalTime();
                itemLC.IdentificadorViagem = identificadorViagem;
                itemLC.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemLC.Identificador;
                itemDePara.TipoObjeto = "LC";
                if (itemLC.Identificador < 0)
                    itemLC.Identificador = null;
                SalvarListaCompra(itemLC);
                itemDePara.IdentificadorDetino = itemLC.Identificador;
                listaResultado.Add(itemDePara);
            }
        }

        private void SincronizarCalendariosPrevistos(ClasseSincronizacao itemDados, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemCP in itemDados.CalendariosPrevistos)
            {
                var itemAporteBase = SelecionarCalendarioPrevisto(itemCP.Identificador);
                if (itemAporteBase != null &&  itemAporteBase.DataAtualizacao > itemCP.DataAtualizacao || itemCP.DataExclusao.HasValue)
                    continue;
                if (itemCP.DataExclusao.HasValue)
                    itemCP.DataExclusao = DateTime.Now.ToUniversalTime();
                itemCP.IdentificadorViagem = identificadorViagem;
                itemCP.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemCP.Identificador;
                itemDePara.TipoObjeto = "CP";
                if (itemCP.Identificador < 0)
                    itemCP.Identificador = null;
                SalvarCalendarioPrevisto(itemCP);
                itemDePara.IdentificadorDetino = itemCP.Identificador;
                listaResultado.Add(itemDePara);
            }
        }

        private void SincronizarComentarios(ClasseSincronizacao itemDados, int identificadorUsuario, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemCP in itemDados.Comentarios)
            {
                var itemAporteBase = SelecionarComentario( itemCP.Identificador);
                if (itemAporteBase != null && itemAporteBase.DataAtualizacao > itemCP.DataAtualizacao || itemCP.DataExclusao.HasValue)
                    continue;
                if (itemCP.DataExclusao.HasValue)
                    itemCP.DataExclusao = DateTime.Now.ToUniversalTime();
                itemCP.IdentificadorViagem = identificadorViagem;
                itemCP.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemCP.IdentificadorUsuario = identificadorUsuario;
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemCP.Identificador;
                itemDePara.TipoObjeto = "T";
                if (itemCP.Identificador < 0)
                    itemCP.Identificador = null;
                itemCP.IdentificadorCidade = RetornarCidadeGeocoding(itemCP.Latitude, itemCP.Longitude);

                SalvarComentario(itemCP);
                itemDePara.ItemRetorno = SelecionarComentario(itemCP.Identificador);
                itemDePara.IdentificadorDetino = itemCP.Identificador;
                listaResultado.Add(itemDePara);
            }
        }

        private void SincronizarSugestoes(ClasseSincronizacao itemDados, int? identificadorViagem, List<DeParaIdentificador> listaResultado)
        {
            foreach (var itemCP in itemDados.Sugestoes)
            {
                var itemAporteBase = SelecionarSugestao(itemCP.Identificador);
                if (itemAporteBase != null &&  itemAporteBase.DataAtualizacao > itemCP.DataAtualizacao || itemCP.DataExclusao.HasValue)
                    continue;
                if (itemCP.DataExclusao.HasValue)
                    itemCP.DataExclusao = DateTime.Now.ToUniversalTime();
                itemCP.IdentificadorViagem = identificadorViagem;
                itemCP.DataAtualizacao = DateTime.Now.ToUniversalTime();
                DeParaIdentificador itemDePara = new DeParaIdentificador();
                itemDePara.IdentificadorOrigem = itemCP.Identificador;
                itemDePara.TipoObjeto = "S";
                if (itemCP.Identificador < 0)
                    itemCP.Identificador = null;
                itemCP.IdentificadorCidade = RetornarCidadeGeocoding(itemCP.Latitude, itemCP.Longitude);

                SalvarSugestao(itemCP);
                itemDePara.ItemRetorno = SelecionarSugestao(itemCP.Identificador);
                itemDePara.IdentificadorDetino = itemCP.Identificador;
                listaResultado.Add(itemDePara);
            }
        }
    }
}
