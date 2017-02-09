using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Configuration
{
    public class OAuthWebConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("acceptedRedirectUrl", IsRequired = true)]
        public string AcceptedRedirectUrl
        {
            get { return base["acceptedRedirectUrl"].ToString(); }
        }

        [ConfigurationProperty("failedRedirectUrl", IsRequired = true)]
        public string FailedRedirectUrl
        {
            get { return base["failedRedirectUrl"].ToString(); }
        }

        [ConfigurationProperty("proxy", IsRequired = false)]
        public string Proxy
        {
            get { return base["proxy"].ToString(); }
        }
    }
}
