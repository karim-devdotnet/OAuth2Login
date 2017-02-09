using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login
{
    internal class OAuth2Util
    {
        public string GetNonce()
        {
            var rand = new Random();
            int nonce = rand.Next(int.MaxValue);
            return nonce.ToString();
        }

        public string GetTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public string GetSha1Signature(string httpMethod, string url, string data, string consumerSecretKey, string requestTokenSecretKey = null)
        {
            var sigBaseString = httpMethod + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(data);
            string signature = GetSignature(sigBaseString, consumerSecretKey, requestTokenSecretKey);

            return signature;
        }

        private string GetSignature(string sigBaseString, string consumerSecretKey, string requestTokenSecretKey = null)
        {
            var signingKey = string.Format("{0}&{1}", consumerSecretKey, !string.IsNullOrEmpty(requestTokenSecretKey) ? requestTokenSecretKey : "");
            var byteKey = Encoding.ASCII.GetBytes(signingKey);

            using (var myhmacsha1 = new HMACSHA1(byteKey))
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(sigBaseString);
                using (var stream = new MemoryStream(byteArray))
                {
                    byte[] hashValue = myhmacsha1.ComputeHash(stream);
                    var hash = Convert.ToBase64String(hashValue);
                    return hash;
                }
            }
        }
    }
}
