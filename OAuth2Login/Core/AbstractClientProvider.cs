using OAuth2Login.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Core
{
    public abstract class AbstractClientProvider
    {
        protected AbstractClientProvider()
        {
        }

        protected AbstractClientProvider(OAuthWebConfigurationElement ccRoot, OAuthConfigurationElement ccOauth)
        {
            ClientId = ccOauth.ClientId;
            ClientSecret = ccOauth.ClientSecret;
            CallBackUrl = ccOauth.CallbackUrl;
            Scope = ccOauth.Scope;
            Endpoint = ccOauth.Endpoint;

            AcceptedRedirectUrl = ccRoot.AcceptedRedirectUrl;
            FailedRedirectUrl = ccRoot.FailedRedirectUrl;
            Proxy = ccRoot.Proxy;
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallBackUrl { get; set; }
        public string Scope { get; set; }
        public string Endpoint { get; set; }

        public string AcceptedRedirectUrl { get; set; }
        public string FailedRedirectUrl { get; set; }
        public string Proxy { get; set; }

        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }
}
