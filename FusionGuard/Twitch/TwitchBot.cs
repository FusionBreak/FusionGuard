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
using FusionGuard.Resources;
using FusionGuard.Database;

namespace FusionGuard.Twitch
{
    internal class TwitchBot : IDisposable
    {
        TwitchClient _client;
        IMediator _mediator;
        string _defaultChannelName;
        BotContext _database;

        public TwitchBot(Config config, TwitchClient client, IMediator mediator, BotContext database)
        {
            _mediator = mediator;
            _database = database;
            _defaultChannelName = config.TwitchUsername;
            _client = client;
            _client.Initialize(new ConnectionCredentials(config.TwitchUsername, config.OAuthKey), config.TwitchUsername);
            _client.AddChatCommandIdentifier('!');
            RegisterEvents();
        }

        public async Task RunAsync()
        {
            _client.Connect();
            await JoinChannelsFromDb();
            await Task.Delay(-1);
        }

        private async Task JoinChannelsFromDb()
        {
            foreach (var user in _database.Users.Where(user => user.Active))
                if (user is not null)
                    _client.JoinChannel(user.Channel);

            await Task.CompletedTask;
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
                _client.SendMessage(GetUsernameFromLog(e.Data), Language.NeedPermissions);
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
                    await _mediator.Send(new CommandHandler.Panic.Command(e.Command.ChatMessage));
                    break;
                case "peace":
                    await _mediator.Send(new Peace.Command(e.Command.ChatMessage));
                    break;
                default:
                    break;
            }
        }

        private string GetUsernameFromLog(string data) => string.Concat(data.SkipWhile(c => c != '#').Skip(1).TakeWhile(c => c != ' '));
    }
}
