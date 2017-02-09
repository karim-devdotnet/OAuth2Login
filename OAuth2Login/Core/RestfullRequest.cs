using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Core
{
    public class RestfullRequest
    {
        public static string Request(string url, string method, string contentType, NameValueCollection header,
            string data, string proxyAddress = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;
            if (header != null)
                request.Headers.Add(header);

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                IWebProxy proxy = new WebProxy(proxyAddress);
                proxy.Credentials = new NetworkCredential();
                request.Proxy = proxy;
            }

            if (!string.IsNullOrEmpty(data))
            {
                using (var swt = new StreamWriter(request.GetRequestStream()))
                {
                    swt.Write(data);
                }
            }

            string result = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
