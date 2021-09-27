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
            _client.AddChatCommandIdentifier('!');
            RegisterEvents();
        }

        public async Task RunAsync()
        {
            _client.Connect();
            await Task.Delay(-1);
        }

        private void RegisterEvents()
        {
            _client.OnChatCommandReceived += _client_OnChatCommandReceived;
            _client.OnNoPermissionError += _client_OnNoPermissionError;
            _client.OnLog += _client_OnLog;
        }

        private void _client_OnLog(object? sender, OnLogArgs e)
        {
            if(e.Data.Contains("@msg-id=no_permission"))
            {
                _client.SendMessage(GetUsernameFromLog(e.Data), "Ich muss zuerst Moderator-Rechte erhalten um dies auszuführen.");
            }

            Console.WriteLine($"{e.DateTime} :\t{e.Data}");
        }

        public void Dispose()
        {
            _client.OnChatCommandReceived -= _client_OnChatCommandReceived;
            _client.OnNoPermissionError -= _client_OnNoPermissionError;
        }

        private void _client_OnNoPermissionError(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
        } 

        private async void _client_OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
        {
            switch(e.Command.CommandText)
            {
                case "ping":
                    await _mediator.Send(new Ping.Command(e.Command.ChatMessage.Channel));
                    break;
                case "join":
                    await _mediator.Send(new Join.Command(e.Command.ChatMessage.Username));
                    break;
                case "leave":
                    await _mediator.Send(new Leave.Command(e.Command.ChatMessage.Username));
                    break;
                case "panic":
                    await _mediator.Send(new Panic.Command(e.Command.ChatMessage));
                    break;
                default:
                    break;
            }
        }

        /*
         * Received: @msg-id=no_permission :tmi.twitch.tv NOTICE #fusionbreak :You don't have permission to perform that action.
         */
        private string GetUsernameFromLog(string data) => string.Concat(data.SkipWhile(c => c != '#').Skip(1).TakeWhile(c => c != ' '));
    }
}
