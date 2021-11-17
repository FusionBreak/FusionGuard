using FusionGuard.Database;
using FusionGuard.Resources;
using FusionGuard.Twitch.Handler;
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
            IMediator _mediator;

            public Handler(TwitchClient client, Dictionary<string, PanicMode> panics, BotContext database, IMediator mediator)
            {
                _client = client;
                _panics = panics;
                _database = database;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                PanicMode panic;
                if(UserIsAuthorized(request) && _panics.TryGetValue(request.ChatMessage.Channel, out panic))
                {
                    var user = _database.Users.Include(user => user.Panics).First(user => user.Channel.ToLower() == request.ChatMessage.Channel);

                    var follower = await _mediator.Send(new GetFollower.Command(user.TwitchUserId));
                    var followerAfterPanicBegin = follower.Where(follow => follow.FollowedAt > panic.Beginn);

                    panic.Stop();
                    user.Panics!.Add(new Database.Panic()
                    {
                        Beginn = panic.Beginn,
                        End = panic.End,
                        PanicFollows = ToPanicFollow(followerAfterPanicBegin).ToArray(),
                    });
                    await _database.SaveChangesAsync();

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

                return await Task.FromResult(Unit.Value);
            }

            private IEnumerable<PanicFollow> ToPanicFollow(IEnumerable<(DateTime FollowedAt, string UserId, string Username)> follows)
            => follows.Select(follow => new PanicFollow()
            {
                FollowedAt = follow.FollowedAt,
                TwitchUserId = follow.UserId,
                Username = follow.Username
            });

            private bool UserIsAuthorized(Command request) 
                => request.ChatMessage.IsBroadcaster || request.ChatMessage.IsModerator;
        }
    }
}
