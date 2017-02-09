using OAuth2Login.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Login.Interface
{
    public interface IClientService
    {
        void CreateOAuthClient(IOAuthContext oContext);
        void CreateOAuthClient(AbstractClientProvider oClient);
    }
}
