using Newtonsoft.Json;
using OAuth2Login.Core;
using System;
using System.Web;

namespace OAuth2Login.Service
{
    internal class GoogleService : BaseOauth2Service
    {
        private static string _oauthUrl = "";

        public GoogleService(AbstractClientProvider oClient, string absoluteUri) : base(oClient, absoluteUri) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.BuildCompex(new[] { "scope" },
                "scope", _client.Scope,
                "state", "1",
                "redirect_uri", $"{_AbsoluteUri.TrimEnd('/')}/{_client.CallBackUrl.TrimStart('/')}",
                "client_id", _client.ClientId,
                "response_type", "code",
                "approval_prompt", "auto",
                "access_type", "online"
                );

            _oauthUrl = "https://accounts.google.com/o/oauth2/auth?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            const string tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var postData = QueryStringBuilder.Build(
                "code", code,
                "client_id", _client.ClientId,
                "client_secret", _client.ClientSecret,
                "redirect_uri", $"{_AbsoluteUri.TrimEnd('/')}/{_client.CallBackUrl.TrimStart('/')}",
                "grant_type", "authorization_code"
                );

            string resonseJson = HttpPost(tokenUrl, postData);
            return JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;
        }

        public override void RequestUserProfile()
        {
            string profileUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + _client.Token;

            string result = HttpGet(profileUrl);

            ParseUserData<GoogleUserData>(result);
        }
    }

    public class GoogleUserData : BaseUserData
    {
        public GoogleUserData() : base(ExternalAuthServices.Google) { }

        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }

        // override
        public override string UserId { get { return id; } }
        public override string Email { get { return email; } }
        public override string FullName { get { return name; } }

        // not implemented
        public override string PhoneNumber { get { return null; } }
    }
}