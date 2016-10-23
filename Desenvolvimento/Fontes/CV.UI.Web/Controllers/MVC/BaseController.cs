using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.UI.Web.Controllers.MVC
{
    public class BaseController : Controller
    {
        // GET: Base
        public BaseController()
        {
            ViewBag.Ambiente = CV.UI.Web.Properties.Settings.Default.Ambiente + " - v" + this.GetType().Assembly.GetName().Version.ToString();
        }
    }
}