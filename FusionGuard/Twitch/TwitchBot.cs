using FusionGuard.Twitch.Commands;
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

namespace FusionGuard.Twitch
{
    public class TwitchBot : IDisposable
    {
        TwitchClient _client;
        string _defaultChannelName;
        DefaultChannelCommandHandler _defaultChannelCommandHandler;

        public bool IsRunning { get; private set; }

        public TwitchBot(string twitchUsername, string oAuthKey, string defaultChannelName)
        {
            _defaultChannelCommandHandler = new DefaultChannelCommandHandler();
            _defaultChannelName = defaultChannelName;
            _client = new TwitchClient(new WebSocketClient(new ClientOptions { MessagesAllowedInPeriod = 750, ThrottlingPeriod = TimeSpan.FromSeconds(30) }));
            _client.Initialize(new ConnectionCredentials(twitchUsername, oAuthKey), defaultChannelName);
            RegisterEvents();
        }

        public void Start()
        {
            _client.Connect();
            IsRunning = true;
        }

        public void Stop()
        {
            _client.Disconnect();
            IsRunning = false;
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

        private void HandleMessageReceived(object? sender, OnMessageReceivedArgs e)
        {         
            if(e.ChatMessage.Channel == _defaultChannelName)
                _defaultChannelCommandHandler.Handle(e.ChatMessage.Message);
        }
    }
}
