using CV.Business;
using CV.Model;
using CV.Model.Dominio;
using CV.UI.Web.Helper;
using CV.UI.Web.Models;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CV.UI.Web.Controllers.WebAPI
{
    public partial class FotoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Foto> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Foto> resultado = new ResultadoConsultaTipo<Foto>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Foto> _itens = biz.ListarFotos(json.Identificador, token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte, json.Comentario, json.ListaAtracoes, json.ListaHoteis, json.ListaRefeicoes, json.IdentificadorCidade, json.Index.Value, json.Count.Value).ToList();
            //resultado.TotalRegistros = _itens.Count();
            //if (json.SortField != null && json.SortField.Any())
            //    _itens = _itens.AsQueryable().OrderByField<Foto>(json.SortField, json.SortOrder).ToList();

            //if (json.Index.HasValue && json.Count.HasValue)
            //    _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }

        [Authorize]
        public Foto Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Foto itemFoto = biz.SelecionarFoto(id);
          
            return itemFoto;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Foto itemFoto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            if (itemFoto.Latitude.HasValue && itemFoto.Longitude.HasValue)
            {
                itemFoto.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemFoto.Latitude, itemFoto.Longitude);
            }
            else
                itemFoto.IdentificadorCidade = null;
            biz.SalvarFoto_Completa(itemFoto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemFoto.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Foto itemFoto = biz.SelecionarFoto_Completa(id);
            itemFoto.DataExclusao = DateTime.Now.ToUniversalTime();
            itemFoto.Atracoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemFoto.Hoteis.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemFoto.Refeicoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemFoto.ItensCompra.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            biz.SalvarFoto_Completa(itemFoto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("RetornarAlbum")]
        public List<string> RetornarAlbum()
        {
            List<string> Dados = new List<string>();
            ViagemBusiness biz = new ViagemBusiness();
            Usuario itemUsuario = biz.SelecionarUsuario(token.IdentificadorUsuario);
            biz.AtualizarTokenUsuario(itemUsuario);
            Viagem itemViagem = biz.SelecionarViagem(token.IdentificadorViagem);
            Dados.Add(itemViagem.CodigoAlbum);
            Dados.Add(itemUsuario.Token);
            return Dados;
        }

        [Authorize]
        [ActionName("SubirImagem")]
        public ResultadoOperacao SubirImagem(UploadFoto itemFoto)
        {
            
            ViagemBusiness biz = new ViagemBusiness();
            Usuario itemUsuario = biz.SelecionarUsuario(token.IdentificadorUsuario);
            return biz.CadastrarFoto(itemFoto, itemUsuario, token.IdentificadorViagem);
        }

        [Authorize]
        [ActionName("SubirImagemDireto")]
        public ResultadoOperacao SubirImagemDireto(UploadFoto itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            Usuario itemUsuario = biz.SelecionarUsuario(token.IdentificadorUsuario);
            return biz.CadastrarFotoDireto(itemFoto, itemUsuario, token.IdentificadorViagem);
        }

        [Authorize]
        [ActionName("SubirVideo")]
        public ResultadoOperacao SubirVideo(UploadFoto itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            if (string.IsNullOrEmpty(itemFoto.ImageMime))
                itemFoto.ImageMime = "video";
            Usuario itemUsuario = biz.SelecionarUsuario(token.IdentificadorUsuario);
            return biz.CadastrarVideo(itemFoto, itemUsuario, token.IdentificadorViagem);
        }

        [Authorize]
        [ActionName("saveFotoAtracao")]
        public void saveFotoAtracao(FotoAtracao itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarFotoAtracao(itemFoto);
        }

        [Authorize]
        [ActionName("saveFotoRefeicao")]
        public void saveFotoRefeicao(FotoRefeicao itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarFotoRefeicao(itemFoto);
        }

        [Authorize]
        [ActionName("saveFotoHotel")]
        public void saveFotoHotel(FotoHotel itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarFotoHotel(itemFoto);
        }

        [Authorize]
        [ActionName("saveFoto")]
        public void saveFoto(Foto itemFoto)
        {

            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarFoto(itemFoto);
        }



    }
}