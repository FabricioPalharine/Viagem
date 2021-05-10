using CV.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.UI.Web.Controllers.MVC
{
    [NoCache]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (!Request.RawUrl.Contains("Index") && !Request.RawUrl.Contains("PoliticaPrivacidade") && !Request.RawUrl.Contains("TermosServico") && !Request.RawUrl.EndsWith("/"))
                return RedirectToAction("Index", "Home");
            return View();
        }

        public ActionResult PoliticaPrivacidade()
        {
            return PartialView();
        }

        public ActionResult TermosServico()
        {
            return PartialView();
        }

        public ActionResult Dashboard()
        {
            return PartialView();
        }

        public ActionResult Alerta()
        {
            return PartialView();
        }

        public ActionResult Login()
        {
            return PartialView();
        }

        public ActionResult AlterarSenha()
        {
            return PartialView();
        }
    }
}