using OAuth2Login.Core;
using System.Collections.Generic;

namespace OAuth2Login.Interface
{
    public interface IOAuthContext
    {
        AbstractClientProvider Client { get; set; }
        IClientService Service { get; set; }

        string Token { get; set; }
        Dictionary<string, string> Profile { get; set; }
        string BeginAuth();
    }
}