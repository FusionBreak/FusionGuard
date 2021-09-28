﻿using FusionGuard.Resources;
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

            public Handler(TwitchClient client)
            {
                _client = client;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if(UserIsAuthorized(request))
                {
                    _client.Marker(request.ChatMessage.Channel);
                    _client.ClearChat(request.ChatMessage.Channel);
                    _client.SubscribersOnlyOn(request.ChatMessage.Channel);
                    _client.SlowModeOn(request.ChatMessage.Channel, TimeSpan.FromMinutes(1));

                    _client.SendMessage(request.ChatMessage.Channel, Language.PanicEnabled);
                }
                
                return Unit.Task;
            }

            private bool UserIsAuthorized(Command request) 
                => request.ChatMessage.IsBroadcaster || request.ChatMessage.IsModerator;
        }
    }
}
