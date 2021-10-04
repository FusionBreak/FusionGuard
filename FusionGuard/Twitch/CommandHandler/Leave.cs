using FusionGuard.Database;
using FusionGuard.Resources;
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
    internal static class Leave
    {
        public record Command(string userName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;
            BotContext _database;

            public Handler(TwitchClient client, BotContext database)
            {
                _client = client;
                _database = database;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if(request.userName == _client.TwitchUsername)
                    return Unit.Task;

                if(_client.JoinedChannels.Any(channel => channel.Channel == request.userName))
                {
                    _client.LeaveChannel(request.userName);
                    _database.Users.First(user => user.Channel == request.userName).Active = false;
                    _database.SaveChanges();
                    _client.SendMessage(_client.TwitchUsername, Language.ChannelLeaved.Replace("{UserName}", request.userName));
                }

                return Unit.Task;
            }
        }
    }
}
