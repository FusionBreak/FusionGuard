using FusionGuard.Configuration;
using FusionGuard.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;

namespace FusionGuard.Twitch.Handler
{
    public class DeleteFollower
    {
        public record Command(string TwitchUserId, IEnumerable<string> FollowerIds) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchAPI _api;
            BotContext _database;

            public Handler(TwitchAPI api, BotContext database)
            {
                _api = api;
                _database = database;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException("DeleteUserFollows not allowed. https://discuss.dev.twitch.tv/t/deprecation-of-create-and-delete-follows-api-endpoints/32351");
                
                //var databaseUser = _database.Users.Include(user => user.AccessToken).First(user => user.TwitchUserId == request.TwitchUserId);
                //var accessToken = databaseUser.AccessToken!.Last().Token;

                //var deletes = request.FollowerIds
                //        .Select(followerId => _api.Helix.Users.DeleteUserFollows(followerId, request.TwitchUserId, accessToken))
                //        .ToArray();

                //await Task.WhenAll(deletes);

                //return await Unit.Task;
            }
        }
    }
}
