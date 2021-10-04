using FusionGuard.Database;
using FusionGuard.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace FusionGuard.Twitch.CommandHandler
{
    internal class Peace
    {
        public record Command(ChatMessage ChatMessage) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;
            Dictionary<string, PanicMode> _panics;
            BotContext _database;

            public Handler(TwitchClient client, Dictionary<string, PanicMode> panics, BotContext database)
            {
                _client = client;
                _panics = panics;
                _database = database;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                PanicMode panic;
                if(UserIsAuthorized(request) && _panics.TryGetValue(request.ChatMessage.Channel, out panic))
                {
                    var user = _database.Users.Include(user => user.Panics).First(user => user.Channel == request.ChatMessage.Channel);
                    user.Panics!.Add(new Database.Panic() 
                    { 
                        Beginn = panic.Beginn,
                        End = DateTime.Now
                    });

                    _database.SaveChanges();


                    if (panic.EmoteOnly is null || (bool)panic.EmoteOnly)
                        _client.EmoteOnlyOff(request.ChatMessage.Channel);

                    if(panic.SubOnly is null || (bool)panic.SubOnly)
                        _client.SubscribersOnlyOff(request.ChatMessage.Channel);

                    if(panic.SlowMode is not null && (int)panic.SlowMode > 0)
                        _client.SlowModeOn(request.ChatMessage.Channel, TimeSpan.FromSeconds((int)panic.SlowMode));
                    else
                        _client.SlowModeOff(request.ChatMessage.Channel);

                    _panics.Remove(request.ChatMessage.Channel);
                    _client.SendMessage(request.ChatMessage.Channel, Language.PeaceEnabled);
                }
                
                return Unit.Task;
            }

            private bool UserIsAuthorized(Command request) 
                => request.ChatMessage.IsBroadcaster || request.ChatMessage.IsModerator;
        }
    }
}
