using OAuth2Login.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Core
{
    public class Oauth2LoginFactory
    {
        public static T CreateClient<T>(string configName) where T : AbstractClientProvider, new()
        {
            if (String.IsNullOrEmpty(configName))
                throw new Exception("Invalid configuration name");

            var ccRoot =
                ConfigurationManager.GetSection("oauth2.login.configuration") as OAuthConfigurationSection;

            if (ccRoot != null)
            {
                var ccWebElem = ccRoot.WebConfiguration;

                IEnumerator configurationReader = ccRoot.OAuthVClientConfigurations.GetEnumerator();

                OAuthConfigurationElement ccOauth = null;
                while (configurationReader.MoveNext())
                {
                    var currentOauthElement = configurationReader.Current as OAuthConfigurationElement;
                    if (currentOauthElement != null && currentOauthElement.Name == configName)
                    {
                        ccOauth = currentOauthElement;
                        break;
                    }
                }

                if (ccOauth != null)
                {
                    var constructorParams = new object[]
                    {
                        ccWebElem,
                        ccOauth
                    };
                    var client = (T)Activator.CreateInstance(typeof(T), constructorParams);

                    return client;
                }
                else
                {
                    throw new Exception("ERROR: [MultiOAuthFactroy] ConfigurationName is not found!");
                }

            }

            return default(T);
        }

    }
}
