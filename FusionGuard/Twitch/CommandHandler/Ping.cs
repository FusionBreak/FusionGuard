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
        public record Command(TwitchClient client, string channelName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                request.client.SendMessage(request.channelName, "Pong!");
                return Unit.Task;
            }
        }
    }
}
