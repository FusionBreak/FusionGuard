using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FusionGuard.Configuration;
using FusionGuard.Database;
using FusionGuard.Twitch;
using FusionGuard.Twitch.Handler;
using FusionGuard.Twitch.WebHandler;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
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
            services.AddSingleton(new TwitchAPI());
            services.AddSingleton(new TwitchClient(new WebSocketClient(new ClientOptions { MessagesAllowedInPeriod = 750, ThrottlingPeriod = TimeSpan.FromSeconds(30) })));
            services.AddSingleton<TwitchBot>();
            services.AddSingleton(new Dictionary<string, PanicMode>());
            _serviceScope= services.BuildServiceProvider().CreateScope();
        }

        private async Task RunAsync()
        {
            var bot = _serviceScope.ServiceProvider.GetRequiredService<TwitchBot>();
            var api = _serviceScope.ServiceProvider.GetRequiredService<TwitchAPI>();
            var config = _serviceScope.ServiceProvider.GetRequiredService<Config>();
            api.Settings.ClientId = config.ClientId;
            var app = WebApplication.CreateBuilder().Build();
            SetupWebApp(app);

            await Task.WhenAll(app.RunAsync(), bot.RunAsync());
        }

        private void SetupWebApp(WebApplication app)
        {
            var mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();

            app.UseHttpsRedirection();

            app.MapGet("/", async (HttpContext context) =>
            {
                return "online";
            });

            app.MapGet("/Authenticate", async (HttpContext context) =>
            {
                var url = await mediator.Send(new GetAuthorizationCodeUrl.Command(new AuthScopes[]
                {
                    AuthScopes.Helix_User_Edit_Follows,
                    AuthScopes.User_Follows_Edit,
                    AuthScopes.Helix_User_Read_BlockedUsers,
                    AuthScopes.User_Read,
                    AuthScopes.Helix_User_Read_Subscriptions,
                    AuthScopes.Helix_Bits_Read
                }));

                context.Response.Redirect(url, true);
            });

            app.MapGet("/TwitchAuthKey", async (HttpContext context) =>
            {
                var mediatr = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();

                var authCode = context.Request.Query["code"];
                var accesToken = await mediatr.Send(new GetAccessToken.Command(authCode));

                return await mediatr.Send(new RegisterUser.Command(accesToken));
            });
        }
    }
}