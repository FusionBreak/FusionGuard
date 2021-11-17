using FusionGuard.Configuration;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;

namespace FusionGuard.Twitch.Handler
{
    public class GetAuthorizationCodeUrl
    {
        public record Command(IEnumerable<AuthScopes> Scopes) : IRequest<string>;

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
                var url = _api.Auth.GetAuthorizationCodeUrl(
                    redirectUri: _config.HostURL + "/TwitchAuthKey",
                    scopes: request.Scopes,
                    forceVerify: true
                );

                return await Task.FromResult(url);
            }
        }
    }
}
