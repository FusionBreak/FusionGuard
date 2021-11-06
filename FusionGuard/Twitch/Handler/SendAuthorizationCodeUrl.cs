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
        public record Command(string Url) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;

            public Handler(TwitchClient client)
            {
                _client = client;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var whisper = Language.AuthorizationCodeUrlSend
                                .Replace("{URL}", request.Url);

                _client.SendMessage(_client.TwitchUsername, whisper);

                return await Unit.Task;
            }
        }
    }
}
