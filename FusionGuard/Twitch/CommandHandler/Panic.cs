using FusionGuard.Resources;
using MediatR;
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
    internal class Panic
    {
        public record Command(ChatMessage ChatMessage) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            TwitchClient _client;
            Dictionary<string, PanicMode> _panics;

            public Handler(TwitchClient client, Dictionary<string, PanicMode> panics)
            {
                _client = client;
                _panics = panics;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if(UserIsAuthorized(request) && !_panics.ContainsKey(request.ChatMessage.Channel))
                {
                    var channelState = _client.GetJoinedChannel(request.ChatMessage.Channel).ChannelState;
                    if(channelState is not null)
                    {
                        var panic = new PanicMode()
                        {
                            EmoteOnly = channelState.EmoteOnly,
                            FollowersOnly = channelState.FollowersOnly,
                            SlowMode = channelState.SlowMode,
                            SubOnly = channelState.SubOnly
                        };

                        _panics.Add(request.ChatMessage.Channel, panic);
                    }
                    else
                    {
                        _panics.Add(request.ChatMessage.Channel, new PanicMode());
                    }

                    _client.Marker(request.ChatMessage.Channel);
                    _client.ClearChat(request.ChatMessage.Channel);
                    _client.SubscribersOnlyOn(request.ChatMessage.Channel);
                    _client.SlowModeOn(request.ChatMessage.Channel, TimeSpan.FromMinutes(1));

                    _client.SendMessage(request.ChatMessage.Channel, Language.PanicEnabled);
                }

                return await Task.FromResult(Unit.Value);
            }

            private bool UserIsAuthorized(Command request) 
                => request.ChatMessage.IsBroadcaster || request.ChatMessage.IsModerator;
        }
    }
}
