using FusionGuard.Configuration;
using FusionGuard.Twitch;

var config = ConfigReader.Read();

using var bot = new TwitchBot(config.TwitchUsername, config.OAuthKey, config.TwitchUsername))
bot.Start();
while(bot.IsRunning)
{

}