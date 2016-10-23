using CV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CV.UI.Web.Helper
{
    public interface IProvidePrincipal
    {
        IPrincipal CreatePrincipal(AuthenticationToken token);
    }
}