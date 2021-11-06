using System;

namespace FusionGuard.Database
{
    public class AccessToken
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? Scope { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int ExpiresIn { get; set; }
    }
}
