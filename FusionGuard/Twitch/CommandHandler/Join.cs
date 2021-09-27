using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FusionGuard.Twitch.CommandHandler
{
    internal static class Join
    {
        public record Command(string channelName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Test");
                
                return Unit.Task;
            }
        }
    }
}
