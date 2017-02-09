using System;
using System.Web;
using OAuth2Login.Core;
using System.Collections.Generic;

namespace OAuth2Login.Service
{
    internal class TwitterService : BaseOauth2Service
    {
        private readonly OAuth2Util _util = new OAuth2Util();

        public TwitterService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                // Twitter relies on Callback URL setting on http://apps.twitter.com/
                // If you set client callback here it'll return 401
                //                "oauth_callback", _client.CallBackUrl,
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", _util.GetNonce(),
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", _util.GetTimeStamp(),
                "oauth_version", "1.0"
                );

            const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
            var signature = _util.GetSha1Signature("POST", requestTokenUrl, qstring, _client.ClientSecret);
            var responseText = HttpPost(requestTokenUrl, qstring + "&oauth_signature=" + Uri.EscapeDataString(signature));

            var twitterAuthResp = new TwitterAuthResponse(responseText);

            const string authenticateUrl = "https://api.twitter.com/oauth/authenticate";
            var oauthUrl = authenticateUrl + "?oauth_token=" + twitterAuthResp.OAuthToken;

            return oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var oauthToken = request["oauth_token"];
            var oauthVerifier = request["oauth_verifier"];

            if (String.IsNullOrEmpty(oauthToken) || String.IsNullOrEmpty(oauthVerifier))
                return OAuth2Consts.ACCESS_DENIED;

            string postData = QueryStringBuilder.Build(
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", _util.GetNonce(),
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", _util.GetTimeStamp(),
                "oauth_token", oauthToken,
                "oauth_verifier", oauthVerifier,
                "oauth_version", "1.0"
                );

            const string accessTokenUrl = "https://api.twitter.com/oauth/access_token";
            var signature = _util.GetSha1Signature("POST", accessTokenUrl, postData, _client.ClientSecret);
            var responseText = HttpPost(accessTokenUrl, postData + "&oauth_signature=" + Uri.EscapeDataString(signature));

            var twitterAuthResp = new TwitterAuthResponse(responseText);

            _client.Token = twitterAuthResp.OAuthToken;
            _client.TokenSecret = twitterAuthResp.OAuthTokenSecret;

            return twitterAuthResp.OAuthToken;
        }

        public override void RequestUserProfile()
        {
            const string callingUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";

            var qstring = generatePayload("GET", callingUrl);
            string result = HttpGet(callingUrl + "?" + qstring);

            ParseUserData<TwitterUserData>(result);
        }

        private string generatePayload(string mode, string callingUrl, params object[] pars)
        {
            var paramsList = new Dictionary<string, object>
            {
                {"oauth_consumer_key", _client.ClientId},
                {"oauth_nonce", _util.GetNonce()},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", _util.GetTimeStamp()},
                {"oauth_token", _client.Token},
                {"oauth_version", "1.0"},
                {"include_email", true}
            };

            for (int i = 0; i < pars.Length; i += 2)
            {
                var key = pars[i];
                var val = pars[i + 1];

                paramsList.Add((string)key, val);
            }

            var qstring = QueryStringBuilder.BuildFromDictionary(paramsList, true);

            var signature = _util.GetSha1Signature(mode, callingUrl, qstring, _client.ClientSecret, _client.TokenSecret);
            qstring += "&oauth_signature=" + Uri.EscapeDataString(signature);

            return qstring;
        }
    }

    public class TwitterUserData : BaseUserData
    {
        public TwitterUserData() : base(ExternalAuthServices.Twitter) { }

        public long id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string profile_location { get; set; }
        public string description { get; set; }
        public string url { get; set; }

        public bool Protected { get; set; }
        public bool verified { get; set; }

        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public int favourites_count { get; set; }
        public int statuses_count { get; set; }

        public bool following { get; set; }
        public bool following_request_sent { get; set; }
        public bool notifications { get; set; }

        // override
        public override string UserId { get { return id_str; } }
        public override string FullName { get { return name; } }

        // not shared by twitter
        public override string Email { get { return null; } }
        public override string PhoneNumber { get { return null; } }
    }

    internal class TwitterAuthResponse
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public bool OAuthCallbackConfirmed { get; set; }
        public string OAuthAuthorizeUrl { get; set; }
        public long UserId { get; set; }
        public string ScreenName { get; set; }

        public TwitterAuthResponse(string responseText)
        {
            string[] keyValPairs = responseText.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        OAuthToken = splits[1];
                        break;
                    case "oauth_token_secret":
                        OAuthTokenSecret = splits[1];
                        break;
                    case "oauth_callback_confirmed":
                        OAuthCallbackConfirmed = splits[1] == "true";
                        break;
                    case "xoauth_request_auth_url":
                        OAuthAuthorizeUrl = splits[1];
                        break;
                    case "user_id":
                        UserId = long.Parse(splits[1]);
                        break;
                    case "screen_name":
                        ScreenName = splits[1];
                        break;
                }
            }
        }
    }
}