using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;

namespace FusionGuard.Twitch.CommandHandler
{
    internal class Ping
    {
        public record Command(string channelName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;

            public Handler(TwitchClient client)
            {
                _client = client;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if(request.channelName != _client.TwitchUsername)
                    return await Task.FromResult(Unit.Value);

                _client.SendMessage(_client.TwitchUsername, "Pong!");
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}
