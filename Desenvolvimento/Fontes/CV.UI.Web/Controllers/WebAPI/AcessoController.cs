using CV.Business;
using CV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.ServiceModel.Channels;
using CV.Model.Dominio;
using Newtonsoft.Json;

namespace CV.UI.Web.Controllers.WebAPI
{
    public class AcessoController : BaseApiController
    {
       
        [ActionName("ValidarUsuario")]
        public UsuarioLogado ValidarUsuario([FromBody] PocoLogin itemLogin)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var ItemUsuario = biz.ListarUsuario(d => d.Codigo == itemLogin.Codigo).FirstOrDefault();
            UsuarioLogado itemResultado = new UsuarioLogado();
            if (ItemUsuario != null && !string.IsNullOrEmpty(ItemUsuario.RefreshToken) )
            {
                itemResultado.Nome = ItemUsuario.Nome;
                itemResultado.Sucesso = true;
                itemResultado.Codigo = ItemUsuario.Identificador.GetValueOrDefault();
                itemResultado.Cultura = "pt-br";

                //biz.VerificarAlbum(ItemUsuario);
                biz.CarregarViagem(itemResultado, itemLogin.IdentificadorViagem);

                AuthenticationToken token = new AuthenticationToken();
                token.Cultura = "pt-br";
                token.Token = ItemUsuario.Token;
                token.IdentificadorUsuario = ItemUsuario.Identificador.GetValueOrDefault();
                token.IdentificadorViagem = itemResultado.IdentificadorViagem;
                string Texto = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(token);
                Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
                itemResultado.AuthenticationToken = Texto;
            }
            return itemResultado;
        }

        [ActionName("LoginGoogle")]
        [HttpPost]
        public UsuarioLogado LoginGoogle([FromBody] PocoLogin itemLogin)
        {
            ViagemBusiness biz = new ViagemBusiness();
            UsuarioLogado itemResultado = biz.LogarUsuarioGoogle(itemLogin, biz);
            return itemResultado;
        }

    
        [ActionName("SelecionarViagem")]
        [Authorize]
        [HttpPost]
        public UsuarioLogado SelecionarViagem(PocoLogin itemLogin)
        {
            UsuarioLogado itemResultado = new UsuarioLogado();
            ViagemBusiness biz = new ViagemBusiness();
            var itemViagem = biz.SelecionarViagem_IdentificadorUsuario_IdentficadorViagem(token.IdentificadorUsuario, itemLogin.IdentificadorViagem);
            if (itemViagem != null)
            {
                token.IdentificadorViagem = itemLogin.IdentificadorViagem;
                itemResultado.IdentificadorViagem = itemLogin.IdentificadorViagem;
                itemResultado.NomeViagem = itemViagem.Nome;
                itemResultado.PermiteEdicao = itemViagem.IdentificadorUsuario == token.IdentificadorUsuario || biz.ListarParticipanteViagem(d => d.IdentificadorUsuario == token.IdentificadorUsuario && d.IdentificadorViagem == itemLogin.IdentificadorViagem).Any();
                itemResultado.VerCustos = itemResultado.PermiteEdicao || biz.ListarUsuarioGasto(d => d.IdentificadorUsuario == token.IdentificadorUsuario && d.IdentificadorViagem == itemLogin.IdentificadorViagem).Any();
                itemResultado.Aberto = itemViagem.Aberto.GetValueOrDefault();
            }
            else
            {
                token.IdentificadorViagem = null;
                
            }
            token.IdentificadorViagem = itemLogin.IdentificadorViagem;
            string Texto = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(token);
            Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
            itemResultado.AuthenticationToken = Texto;

            return itemResultado;
        }


        [ActionName("LoginAplicativo")]
        [HttpPost]
        public UsuarioLogado LoginAplicativo([FromBody] DadosGoogleToken itemLogin)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.LoginAplicativo(itemLogin);
        }

        [ActionName("CarregarDadosAplicativo")]
        [HttpPost]
        public UsuarioLogado CarregarDadosAplicativo([FromBody] UsuarioLogado itemLogin)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.CarregarDadosAplicativo(itemLogin);
        }

        [ActionName("CarregarAlertas")]
        [Authorize]
        [HttpGet]
        public List<AlertaUsuario> CarregarAlertasUsuario()
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.CarregarAlertasUsuario(token);
        }

       
    }
}