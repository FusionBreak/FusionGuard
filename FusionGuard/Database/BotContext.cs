using FusionGuard.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Database
{
    internal class BotContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<User> Users { get; set; }

        public BotContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BotContext(Config config)
        {
            _connectionString = config.DatabaseConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
    }
}
