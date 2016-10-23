using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using CV.Model;

namespace CV.UI.Web.Helper
{
    public class BasicAuthMessageHandler : DelegatingHandler
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Bearer";

        public IProvidePrincipal PrincipalProvider { get; set; }

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue authValue = request.Headers.Authorization;
            if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                AuthenticationToken parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);
                if (parsedCredentials != null)
                {
                    request.GetRequestContext().Principal =
                    Thread.CurrentPrincipal = PrincipalProvider
                            .CreatePrincipal(parsedCredentials);
                }
            }
            return base.SendAsync(request, cancellationToken)
                 .ContinueWith(task =>
                 {
                     var response = task.Result;
                     if (response.StatusCode == HttpStatusCode.Unauthorized
                             && !response.Headers.Contains(BasicAuthResponseHeader))
                     {
                         response.Headers.Add(BasicAuthResponseHeader
                                 , BasicAuthResponseHeaderValue);
                     }
                     return response;
                 });
        }

        private AuthenticationToken ParseAuthorizationHeader(string authHeader)
        {
            string credentials = CV.Business.Library.UtilitarioBusiness.Descriptografa(authHeader);

            AuthenticationToken token = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<AuthenticationToken>(credentials);
            return token;
        }
    }
}