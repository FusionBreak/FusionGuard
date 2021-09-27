using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace FusionGuard.Twitch.CommandHandler
{
    internal class Panic
    {
        public record Command(TwitchClient client, string channelName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                request.client.ClearChat(request.channelName);
                return Unit.Task;
            }
        }
    }
}
