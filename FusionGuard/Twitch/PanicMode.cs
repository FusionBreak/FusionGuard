using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Twitch
{
    internal class PanicMode
    {
        public bool? SubOnly { get; init; }
        public bool? EmoteOnly {  get; init; }
        public TimeSpan? FollowersOnly { get; init; }
        public int? SlowMode { get; init; }

        public DateTime Beginn {  get; }
        public DateTime End { get; private set; }
        public TimeSpan Duration => Beginn.Subtract(End);

        public PanicMode() => Beginn = DateTime.UtcNow;

        public void Stop() => End = DateTime.UtcNow;
    }
}
