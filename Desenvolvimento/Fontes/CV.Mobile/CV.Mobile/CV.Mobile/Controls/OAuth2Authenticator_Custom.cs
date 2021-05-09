using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace CV.Mobile.Controls
{
    public class OAuth2Authenticator_Custom : OAuth2Authenticator
    {
        public OAuth2Authenticator_Custom(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false) : base(clientId, scope, authorizeUrl, redirectUrl, getUsernameAsync, isUsingNativeUI)
        {
        }

        public OAuth2Authenticator_Custom(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false) : base(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync, isUsingNativeUI)
        {
        }

        public Dictionary<string, string> CustomKeys { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Fragmentos { get; set; } = new Dictionary<string, string>();
        public override Task<Uri> GetInitialUrlAsync(Dictionary<string, string> custom_query_parameters = null)
        {
            return base.GetInitialUrlAsync(CustomKeys);
        }

        protected override void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            Fragmentos = new Dictionary<string, string>(query);
            base.OnRedirectPageLoaded(url, query, fragment);
        }
    }
}
