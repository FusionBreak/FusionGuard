using FusionGuard.Configuration;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Auth;

namespace FusionGuard.Twitch.WebHandler
{
    internal class GetAccessToken
    {
        public record Command(string AuthCode) : IRequest<string>;

        internal class Handler : IRequestHandler<Command, string>
        {
            TwitchAPI _api;
            Config _config;

            public Handler(TwitchAPI api, Config config)
            {
                _api = api;
                _config = config;
            }

            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = await _api.Auth.GetAccessTokenFromCodeAsync(request.AuthCode, _config.ClientSecret, _config.HostURL+ "/TwitchAuthKey");
                return response.AccessToken;
            }
        }
    }
}
