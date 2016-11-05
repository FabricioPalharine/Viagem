using CV.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CV.UI.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configuration.MessageHandlers
                      .Add(new BasicAuthMessageHandler()
                      {
                          PrincipalProvider = new ValidateTokenPrincipalProvider()
                      });
            GlobalConfiguration.Configuration.Services.Clear(typeof(System.Web.Http.Validation.IBodyModelValidator));

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            string lang = "pt-br";
            //if (System.Web.HttpContext.Current.Request.UserLanguages != null)
            //    lang = System.Web.HttpContext.Current.Request.UserLanguages.FirstOrDefault();
            //if (Context.Request.Cookies["lang"] != null)
            //    lang = Context.Request.Cookies["lang"].Value;
            //if (lang.ToLower() != "en-us" && lang.ToLower() != "pt-br")
            //    lang = "pt-br";
            CultureInfo culture = CultureInfo.GetCultureInfo(lang);

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }

    }
}