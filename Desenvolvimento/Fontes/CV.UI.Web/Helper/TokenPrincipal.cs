using CV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CV.UI.Web.Helper
{
    public class TokenPrincipal : IPrincipal
    {
        public AuthenticationToken Token { get; private set; }
        private IIdentity identity;

        public TokenPrincipal(AuthenticationToken token)
        {
            identity = new GenericIdentity(token.IdentificadorUsuario.ToString());
            Token = token;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}