using  CV.Business;
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

namespace CV.UI.Web.Controllers.WebAPI
{
    public class AcessoController : BaseApiController
    {
        [ActionName("Autenticar")]
        [HttpPost]
        public UsuarioLogado Autenticar([FromBody] PocoLogin itemLogin)
        {
            //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
            //string IP = GetClientIp();
            //string nomeMaquina = string.Empty;
            //string ipMaquina = string.Empty;
            //try
            //{
            //    nomeMaquina = (Dns.GetHostEntry(IP).HostName);
            //}
            //catch
            //{
            //    nomeMaquina = IP;
            //}
            //return biz.ExecutarLoginUsuario(itemLogin, IP, nomeMaquina, System.Threading.Thread.CurrentThread.CurrentCulture.Name);

            AuthenticationToken token = new AuthenticationToken();
            token.Cultura = "pt-br";
            token.IdentificadorLogAcesso = 1;
            token.IdentificadorUsuario = 1;
            string Texto = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(token);
            Texto = Business.Library.UtilitarioBusiness.Criptografa(Texto);
            return new UsuarioLogado() {AuthenticationToken = Texto, Sucesso = true, Ativo = true, Codigo = 1, DataHoraAcesso = DateTime.Now, Login = itemLogin.Login, Nome = "Teste", UltimoAcesso = DateTime.Now.AddDays(-1), LogAcesso = 1 } ;
        }



        [ActionName("RetornarAcessos")]
        [Authorize]
        [HttpGet]
        public int[] RetornarAcessos()
        {

        //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
        //return biz.ListarAcessosUsuario(token.IdentificadorUsuario).ToArray();
        return new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };

        }

        [ActionName("AlterarSenha")]
        [Authorize]
        [HttpPost]
        public ResultadoOperacao AlterarSenha([FromBody] PocoLogin itemLogin)
        {

        //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
        //biz.AdicionaLogChamada(token.IdentificadorUsuario, (Funcionalidade.AlterarSenha), (int)(enumTipoAcao.Alterar), true);

        //return biz.AlterarSenhaUsuario(token, itemLogin);
        return new ResultadoOperacao() { Sucesso = true, Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Senha Alterada com Sucesso" } } };
    }



        [ActionName("Logoff")]
        [HttpGet]
        [Authorize]
        public void Logoff()
        {
            //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
            //biz.DesconectarSessao(token);
        }

        [ActionName("VerificarLogado")]
        [HttpPost]
        [Authorize]
        public UsuarioLogado VerificarLogado()
        {
            //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
            //token.Cultura = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            //return biz.RetornarUsuarioToken(token);
            return new UsuarioLogado() { Sucesso = true, Ativo = true, Codigo = 1, DataHoraAcesso = DateTime.Now, Login = "teste", Nome = "Teste", UltimoAcesso = DateTime.Now.AddDays(-1), LogAcesso = 1 };
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}