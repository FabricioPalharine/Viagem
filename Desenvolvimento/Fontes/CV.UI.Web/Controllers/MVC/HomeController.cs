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

            return View();
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