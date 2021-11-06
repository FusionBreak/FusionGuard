using FusionGuard.Database;
using FusionGuard.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;

namespace FusionGuard.Twitch.WebHandler
{
    internal class RegisterUser
    {
        public record Command(string AccessToken) : IRequest<object>;

        internal class Handler : IRequestHandler<Command, object>
        {
            TwitchAPI _api;
            BotContext _database;
            TwitchClient _client;

            public Handler(TwitchAPI api, BotContext database, TwitchClient client)
            {
                _api = api;
                _database = database;
                _client = client;
            }

            public async Task<object> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = await _api.Auth.ValidateAccessTokenAsync(request.AccessToken);
                var channel = await _api.Helix.Channels.GetChannelInformationAsync(response.UserId, request.AccessToken);
                var channelName = channel.Data.First().BroadcasterName;


                if (!_database.Users.Any(user => user.TwitchUserId == response.UserId))
                {
                    await _database.Users.AddAsync(new User()
                    {
                        Active = true,
                        Channel = channelName,
                        TwitchUserId = response.UserId
                    });

                    await _database.SaveChangesAsync();
                }

                var databaseUser = _database.Users.Include(user => user.AccessToken).First(user => user.TwitchUserId == response.UserId);            
                databaseUser.AccessToken!.Add(new AccessToken()
                {
                    Token = request.AccessToken,
                    Scope =  JsonSerializer.Serialize(response.Scopes),
                    RegisteredAt = DateTime.Now,
                    ExpiresIn = response.ExpiresIn,
                });

                await _database.SaveChangesAsync();

                if (!_client.JoinedChannels.Any(channel => channel.Channel == channelName) && channelName != _client.TwitchUsername)
                {
                    _client.JoinChannel(channelName);
                    _client.SendMessage(_client.TwitchUsername, Language.ChannelJoined.Replace("{UserName}", channelName));
                }


                return await Task.FromResult("done");
            }
        }
    }
}
