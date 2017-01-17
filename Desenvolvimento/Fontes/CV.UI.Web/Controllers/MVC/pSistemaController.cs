using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.UI.Web.Controllers.MVC
{
    public partial class SistemaController: BaseController
    {
        public ActionResult GastoSelecao()
        {
            return PartialView();
        }

        public ActionResult CarroDeslocamentoEdicao()
        {
            return PartialView();
        }

        public ActionResult ComprasEdicao()
        {
            return PartialView();
        }

        public ActionResult ItemCompraEdicao()
        {
            return PartialView();
        }

        public ActionResult PedidoCompra()
        {
            return PartialView();
        }

        public ActionResult PedidoCompraEdicao()
        {
            return PartialView();
        }

        public ActionResult VerificarSugestao()
        {
            return PartialView();
        }

        public ActionResult VerificarSugestaoEdicao()
        {
            return PartialView();
        }

        public ActionResult ConsultarExtratoMoeda()
        {
            return PartialView();
        }
    }
}