using FusionGuard.Configuration;
using FusionGuard.Resources;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;

namespace FusionGuard.Twitch.Handler
{
    public class SendAuthorizationCodeUrl
    {
        public record Command() : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;
            Config _config;

            public Handler(TwitchClient client, Config config)
            {
                _client = client;
                _config = config;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var whisper = Language.AuthorizationCodeUrlSend
                                .Replace("{URL}", _config.HostURL + "/Authenticate");

                _client.SendMessage(_client.TwitchUsername, whisper);

                return await Unit.Task;
            }
        }
    }
}
