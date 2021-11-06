using System;

namespace FusionGuard.Database
{
    public class PanicFollow
    {
        public long Id { get; set; }
        public Panic? Panic { get; set; }
        public string Username { get; set; }
        public string TwitchUserId { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
