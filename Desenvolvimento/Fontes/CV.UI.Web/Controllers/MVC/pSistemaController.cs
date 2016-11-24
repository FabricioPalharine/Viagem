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
    }
}