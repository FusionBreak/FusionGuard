using System;
using System.Threading.Tasks;
using FusionGuard.Configuration;
using FusionGuard.Twitch;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<TwitchBot>();
            _serviceScope= services.BuildServiceProvider().CreateScope();
        }

        private async Task RunAsync()
        {
            var bot = _serviceScope.ServiceProvider.GetRequiredService<TwitchBot>();
            await bot.RunAsync();
        }
    }
}