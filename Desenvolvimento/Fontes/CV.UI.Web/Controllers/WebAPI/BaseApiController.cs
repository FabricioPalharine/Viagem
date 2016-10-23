using CV.Model;
using CV.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace CV.UI.Web.Controllers.WebAPI
{
    public abstract class BaseApiController : ApiController
    {

        protected AuthenticationToken token { get; set; }
        public BaseApiController()
        {

        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal is TokenPrincipal)
                token = ((TokenPrincipal)Thread.CurrentPrincipal).Token;
            if (controllerContext.Request.Headers.Contains("LanguageUI"))
            {
                IEnumerable<string> headerValues = controllerContext.Request.Headers.GetValues("LanguageUI");
                var LanguageName = headerValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(LanguageName))
                {
                    var culture = new System.Globalization.CultureInfo(LanguageName); // you may need to interpret the value of "lang" to match what is expected by CultureInfo

                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
            base.Initialize(controllerContext);
        }

    }
}