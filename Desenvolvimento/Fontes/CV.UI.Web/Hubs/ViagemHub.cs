using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using CV.Model;
using CV.Business;

namespace CV.UI.Web.Hubs
{
    [HubName("Viagem")]

    public class ViagemHub : Hub
    {
        private List<UsuarioLogado> UsuariosConetados = new List<UsuarioLogado>();

        public void ConectarUsuario(int IdentificadorUsuario)
        {
            UsuarioLogado itemUsuario = new UsuarioLogado() { AuthenticationToken = Context.ConnectionId, Codigo = IdentificadorUsuario };
            UsuariosConetados.Add(itemUsuario);
           // Clients.All.EnviarAlertaRequisicao("Teste");
        }

        public void DesconectarUsuario(int IdentificadorUsuario)
        {
            var item = UsuariosConetados.FirstOrDefault(x => x.AuthenticationToken == Context.ConnectionId);
            if (item != null)
            {
                UsuariosConetados.Remove(item); // list = 
            }

        }

        public void RequisitarAmizade(int IdentificadorUsuario, int IdentificadorRequisicao)
        {
            var UsuariosConectados = UsuariosConetados.Where(x => x.Codigo == IdentificadorUsuario);
            if (UsuariosConectados.Any())
            {
                var itemRequisitante = UsuariosConetados.FirstOrDefault(x => x.AuthenticationToken == Context.ConnectionId);
                ViagemBusiness biz = new ViagemBusiness();
                var itemUsuarioLogado = biz.SelecionarUsuario(itemRequisitante.Codigo);
                AlertaUsuario itemAlerta = new AlertaUsuario() {  IdentificadorAlerta = IdentificadorRequisicao, MensagemAlerta = MensagemBusiness.RetornaMensagens("AlertaRequisicaoAmizade", new string[] { itemUsuarioLogado.Nome }), TipoAlerta=1 };
                foreach (var itemUsuario in UsuariosConectados)
                    Clients.Client(itemUsuario.AuthenticationToken).EnviarAlertaRequisicao(itemAlerta);
                RequisicaoAmizade itemRequisicao = biz.SelecionarRequisicaoAmizade(IdentificadorRequisicao);
                if (itemRequisicao != null)
                {
                    itemRequisicao.Status = 2;
                    biz.SalvarRequisicaoAmizade(itemRequisicao);
                }
            }
        }

        public void ConectarViagem(int? IdentificadorViagem, bool Edicao)
        {
            var item = UsuariosConetados.FirstOrDefault(x => x.AuthenticationToken == Context.ConnectionId);
            if (item != null)
            {
                item.IdentificadorViagem = IdentificadorViagem;
                item.PermiteEdicao = Edicao;
            }
        }

        public void SugerirVisitaViagem(Sugestao itemSugestao)
        {
            var UsuariosConectados = UsuariosConetados.Where(x => x.IdentificadorViagem == itemSugestao.IdentificadorViagem && x.PermiteEdicao );
            if (UsuariosConectados.Any())
            {
                foreach (var itemUsuario in UsuariosConectados)
                    Clients.Client(itemUsuario.AuthenticationToken).EnviarAlertaSugestao(itemSugestao);
                itemSugestao.Status = 1;
                ViagemBusiness biz = new ViagemBusiness();
                biz.SalvarSugestao(itemSugestao);
            }
        }

        public void ViagemAtualizada(int IdentificadorViagem, string TipoAtualizacao)
        {
            var UsuariosConectados = UsuariosConetados.Where(x => x.IdentificadorViagem == IdentificadorViagem && x.AuthenticationToken != Context.ConnectionId);
            foreach (var itemUsuario in UsuariosConectados)
                Clients.Client(itemUsuario.AuthenticationToken).AvisarAlertaAtualizacao(TipoAtualizacao);
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = UsuariosConetados.FirstOrDefault(x => x.AuthenticationToken == Context.ConnectionId);
            if (item != null)
            {
                UsuariosConetados.Remove(item); // list = 
            }
            return base.OnDisconnected(true);
        }
    }
}