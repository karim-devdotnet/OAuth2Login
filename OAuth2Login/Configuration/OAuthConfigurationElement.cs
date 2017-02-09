using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Configuration
{
    public class OAuthConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return base["name"].ToString(); }
        }

        [ConfigurationProperty("clientid", IsRequired = true)]
        public string ClientId
        {
            get { return base["clientid"].ToString(); }
        }

        [ConfigurationProperty("clientsecret", IsRequired = true)]
        public string ClientSecret
        {
            get { return base["clientsecret"].ToString(); }
        }

        [ConfigurationProperty("callbackUrl", IsRequired = false, DefaultValue = "oob")]
        public string CallbackUrl
        {
            get { return base["callbackUrl"].ToString(); }
        }

        [ConfigurationProperty("scope", IsRequired = false)]
        public string Scope
        {
            get { return base["scope"].ToString(); }
        }

        [ConfigurationProperty("endpoint", IsRequired = false)]
        public string Endpoint
        {
            get { return base["endpoint"].ToString(); }
        }
    }
}
