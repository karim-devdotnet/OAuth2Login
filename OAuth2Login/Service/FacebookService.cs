using System;
using System.Web;
using OAuth2Login.Core;
using Newtonsoft.Json;

namespace OAuth2Login.Service
{
    internal class FacebookService : BaseOauth2Service
    {
        private static string _oauthUrl = "";

        public FacebookService(AbstractClientProvider oClient, string absoluteUri) : base(oClient, absoluteUri) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                "client_id", _client.ClientId,
                "redirect_uri", $"{_AbsoluteUri.TrimEnd('/')}/{_client.CallBackUrl.TrimStart('/')}",
                "scope", _client.Scope,
                "state", ""//,
                           //"display", "popup"
                );

            _oauthUrl = "https://www.facebook.com/dialog/oauth?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            string tokenUrl = "https://graph.facebook.com/oauth/access_token?";
            string postData = QueryStringBuilder.Build(
                "client_id", _client.ClientId,
                "redirect_uri", $"{_AbsoluteUri.TrimEnd('/')}/{_client.CallBackUrl.TrimStart('/')}",
                "client_secret", _client.ClientSecret,
                "code", code
            );

            string resonseJson = HttpPost(tokenUrl, postData);
            resonseJson = "{\"" + resonseJson.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            return JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;
        }

        public override void RequestUserProfile()
        {
            string profileUrl = $"https://graph.facebook.com/me?access_token={_client.Token}&fields=name,first_name,last_name,gender,email";
            string result = HttpGet(profileUrl);

            ParseUserData<FacebookUserData>(result);
        }
    }

    public class FacebookUserData : BaseUserData
    {
        public FacebookUserData() : base(ExternalAuthServices.Facebook) { }

        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public string gender { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public int timezone { get; set; }
        public bool verified { get; set; }

        // override
        public override string UserId { get { return id; } }
        public override string Email { get { return email; } }
        public override string FullName { get { return name; } }

        public override string PhoneNumber { get { return null; } }
    }
}