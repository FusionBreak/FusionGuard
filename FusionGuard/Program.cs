using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FusionGuard.Configuration;
using FusionGuard.Twitch;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace FusionGuard
{
    class Program
    {
        private IServiceScope _serviceScope;

        static void Main() => new Program().RunAsync().Wait();

        private Program()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Program));
            services.AddSingleton(ConfigReader.Read());
            services.AddSingleton(new TwitchClient(new WebSocketClient(new ClientOptions { MessagesAllowedInPeriod = 750, ThrottlingPeriod = TimeSpan.FromSeconds(30) })));
            services.AddSingleton<TwitchBot>();
            services.AddSingleton(new Dictionary<string, PanicMode>());
            _serviceScope= services.BuildServiceProvider().CreateScope();
        }

        private async Task RunAsync()
        {
            var bot = _serviceScope.ServiceProvider.GetRequiredService<TwitchBot>();
            await bot.RunAsync();
        }
    }
}