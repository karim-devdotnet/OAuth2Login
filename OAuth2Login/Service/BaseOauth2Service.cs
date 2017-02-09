using Newtonsoft.Json;
using OAuth2Login.Client;
using OAuth2Login.Core;
using OAuth2Login.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OAuth2Login.Service
{
    public abstract class BaseOauth2Service: IClientService
    {
        protected AbstractClientProvider _client;
        protected string _AbsoluteUri = null;

        protected BaseOauth2Service(AbstractClientProvider oClient, string absoluteUri = null)
        {
            _client = oClient;
            _AbsoluteUri = absoluteUri;
        }

        public void CreateOAuthClient(AbstractClientProvider oClient)
        {
            _client = oClient;
        }

        public void CreateOAuthClient(IOAuthContext oContext)
        {
            _client = oContext.Client;
        }

        protected string HttpGet(string url, string getData = null)
        {
            var header = new NameValueCollection
            {
                {"Accept-Language", "en-US"}
            };

            return RestfullRequest.Request(url, "GET", "application/x-www-form-urlencoded", 
                null,getData, _client.Proxy);
        }

        protected string HttpPost(string urlToPost, string postData)
        {
            var result = RestfullRequest.Request(urlToPost, "POST", "application/x-www-form-urlencoded",
                    null, postData, _client.Proxy);

            return result;
        }

        public TData GetUserInfo<TData>() where TData : BaseUserData
        {
            return UserData as TData;
        }

        public abstract string BeginAuthentication();
        public abstract string RequestToken(HttpRequestBase request);
        public abstract void RequestUserProfile();

        public static BaseOauth2Service GetService(string id, string absoluteUri)
        {
            switch (id.ToLower())
            {
                case "google":
                    return new GoogleService(Oauth2LoginFactory.CreateClient<GoogleClient>("Google"),absoluteUri);
                    break;
                case "facebook":
                    return new FacebookService(Oauth2LoginFactory.CreateClient<FacebookClient>("Facebook"), absoluteUri);
                    break;
                //case "windowslive":
                //    return new WindowsLiveService(Oauth2LoginFactory.CreateClient<WindowsLiveClient>("WindowsLive"));
                //    break;
                case "paypal":
                    return new PayPalService(Oauth2LoginFactory.CreateClient<PayPalClient>("PayPal"), absoluteUri);
                    break;
                case "twitter":
                    return new TwitterService(Oauth2LoginFactory.CreateClient<TwitterClient>("Twitter"));
                    break;
                default:
                    return null;
            }
        }

        public string ValidateLogin(HttpRequestBase request)
        {
            // client token
            string tokenResult = RequestToken(request);
            if (tokenResult == OAuth2Consts.ACCESS_DENIED)
                return _client.FailedRedirectUrl;

            _client.Token = tokenResult;

            // client profile
            RequestUserProfile();

            UserData.OAuthToken = _client.Token;
            UserData.OAuthTokenSecret = _client.TokenSecret;

            return null;
        }

        public void ImpersonateUser(string oauthToken, string oauthTokenSecret)
        {
            _client.Token = oauthToken;
            _client.TokenSecret = oauthTokenSecret;
        }

        protected void ParseUserData<TData>(string json) where TData : BaseUserData
        {
            UserDataJsonSource = json;
            UserData = ParseJson<TData>(json);
        }

        protected T ParseJson<T>(string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, (T)Activator.CreateInstance(typeof(T)));
        }

        public BaseUserData UserData { get; set; }
        public string UserDataJsonSource { get; set; }
    }
}
