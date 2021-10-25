using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FusionGuard.Configuration;
using FusionGuard.Database;
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

        static void Main(string[] args) => new Program(args).RunAsync().Wait();

        private Program(string[] args)
        {            
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Program));
            services.AddSingleton(args.Length > 0 ? ConfigReader.ReadCliArgs(args) : ConfigReader.ReadEnvVars());
            services.AddDbContext<BotContext>();
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