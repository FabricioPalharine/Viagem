using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace CV.Mobile.Services
{
    public class OAuth2AuthenticatorToken : OAuth2Authenticator
    {
        private Uri AuthorizeUrl1;
        private Uri RedirectUrl;
        private string Scope1;

        public OAuth2AuthenticatorToken(string clientId, string clientSecret, string scope, Uri authorizeURL, Uri redirectUrl, Uri UserTokenURL) :
            base(clientId, clientSecret, scope, authorizeURL, redirectUrl, UserTokenURL,null,true)
        {
            AuthorizeUrl1 = authorizeURL;
            RedirectUrl = redirectUrl;
            Scope1 = scope;
        }

        public override Task<Uri> GetInitialUrlAsync()
        {
            string uriString = string.Format(
                "{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}&access_type=offline&approval_prompt=force",
                this.AuthorizeUrl1.AbsoluteUri,
                Uri.EscapeDataString(this.ClientId),
                Uri.EscapeDataString(this.RedirectUrl.AbsoluteUri),
                "code",
                Uri.EscapeDataString(this.Scope1)
            );

            var url = new Uri(uriString);
            return Task.FromResult(url);
        }
    }
}
