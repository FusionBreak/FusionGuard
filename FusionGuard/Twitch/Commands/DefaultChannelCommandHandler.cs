using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Twitch.Commands
{
    public class DefaultChannelCommandHandler
    {
        public Action OnPanik { get; set; }

        public void Handle(string message)
        {
            if(!message.StartsWith("!"))
                return;

            var command = ChatCommand.Parse(message);

            switch (command.Command.ToLower())
            {
                case "ping":
                    Console.WriteLine("Pong!");
                    break;
                case "panik":
                    OnPanik?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}
