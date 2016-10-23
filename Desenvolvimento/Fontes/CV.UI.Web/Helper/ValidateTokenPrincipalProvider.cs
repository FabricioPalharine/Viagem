using CV.Business;
using CV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CV.UI.Web.Helper
{
    public class ValidateTokenPrincipalProvider : IProvidePrincipal
    {
        public IPrincipal CreatePrincipal(AuthenticationToken token)
        {
            //GerenciadorAcessoBusiness biz = new GerenciadorAcessoBusiness();
            //if (biz.ValidarAuthenticationToken(token))
            //{

                IPrincipal principal = new TokenPrincipal(token);
                return principal;
            //}
            //else
            //    return null;
        }
    }
}