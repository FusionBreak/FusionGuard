using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace FusionGuard.Twitch
{
    public class TwitchBot : IDisposable
    {
        TwitchClient _client;

        public bool IsRunning { get; private set; }

        public TwitchBot(string twitchUsername, string oAuthKey, string defaultChannelName)
        {
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
            _client.OnLog += Client_OnLog;
        }

        public void Dispose()
        {
            _client.OnLog -= Client_OnLog;
        }

        private void Client_OnLog(object? sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }
    }
}
