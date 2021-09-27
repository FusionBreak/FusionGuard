using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Twitch
{
    public class ChatCommand
    {
        public string Command => _command;
        public string[] Parameter => _parameter;

        readonly string _command;
        readonly string[] _parameter;

        private ChatCommand(string command, string[] parameter)
        {
            _command = command;
            _parameter = parameter;
        }

        public static ChatCommand Parse(string message)
        {
            var parts = message.Remove(0, 1).Split(' ');
            return new ChatCommand(parts[0], parts[1..]);
        }
    }
}
