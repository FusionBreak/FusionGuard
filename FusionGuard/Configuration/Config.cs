using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Configuration
{
    public record Config(string TwitchUsername, string OAuthKey, string ClientId, string ClientSecret, string DatabaseConnectionString, string HostURL, string EncryptionKey);
}
