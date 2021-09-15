using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FusionGuard.Configuration
{
    public static class ConfigReader
    {
        public static Config Read()
        {
            var config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

            return new Config(config.GetSection("TwitchUsername").Value, config.GetSection("OAuthKey").Value);
        }
    }
}
