using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Extensions;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using FusionGuard.Configuration;
using MediatR;
using FusionGuard.Twitch.CommandHandler;

namespace FusionGuard.Twitch
{
    public class TwitchBot : IDisposable
    {
        TwitchClient _client;
        IMediator _mediator;
        string _defaultChannelName;

        public TwitchBot(Config config, TwitchClient client, IMediator mediator)
        {
            _mediator = mediator;
            _defaultChannelName = config.TwitchUsername;
            _client = client;
            _client.Initialize(new ConnectionCredentials(config.TwitchUsername, config.OAuthKey), config.TwitchUsername);
            RegisterEvents();
        }

        public async Task RunAsync()
        {
            _client.Connect();
            await Task.Delay(-1);
        }

        private void RegisterEvents()
        {
            _client.OnLog += Log;
            _client.OnMessageReceived += HandleMessageReceived;
        }

        public void Dispose()
        {
            _client.OnLog -= Log;
            _client.OnMessageReceived -= HandleMessageReceived;
        }

        private void Log(object? sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }

        private async void HandleMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Channel == _defaultChannelName && e.ChatMessage.Message.StartsWith("!"))
            {
                var chatCommand = ChatCommand.Parse(e.ChatMessage.Message);

                switch (chatCommand.Command.ToLower())
                {
                    case "ping":
                        await _mediator.Send(new Ping.Command(e.ChatMessage.Channel));
                        break;
                    case "panic":
                        await _mediator.Send(new Panic.Command(_client, e.ChatMessage.Channel));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
