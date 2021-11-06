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
    public class GetFollower
    {
        public record Command(string TwitchUserId) : IRequest<IEnumerable<(DateTime FollowedAt, string UserId, string Username)>>;

        internal class Handler : IRequestHandler<Command, IEnumerable<(DateTime, string, string)>>
        {
            TwitchAPI _api;
            BotContext _database;

            public Handler(TwitchAPI api, BotContext database)
            {
                _api = new TwitchAPI();
                _api.Settings.ClientId = api.Settings.ClientId;

                _database = database;
            }

            public async Task<IEnumerable<(DateTime, string, string)>> Handle(Command request, CancellationToken cancellationToken)
            {
                var databaseUser = _database.Users.Include(user => user.AccessToken).First(user => user.TwitchUserId == request.TwitchUserId);
                _api.Settings.AccessToken = databaseUser.AccessToken!.Last().Token;

                return await GetAllFollower(_api, request.TwitchUserId);
            }

            private async Task<List<(DateTime, string, string)>> GetAllFollower(TwitchAPI api, string id, string page = null)
            {
                var follower = new List<(DateTime, string, string)>();

                var response = await api.Helix.Users.GetUsersFollowsAsync(toId: id, after: page, first: 100);
                var followerInPage = response.Follows.Select(follow => new Tuple<DateTime, string, string>(follow.FollowedAt, follow.FromUserId, follow.FromUserName).ToValueTuple());
                follower.AddRange(followerInPage);

                if (!string.IsNullOrEmpty(response.Pagination.Cursor))
                    follower.AddRange(await GetAllFollower(api, id, response.Pagination.Cursor));

                return follower;
            }
        }
    }
}
