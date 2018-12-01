using System;
using JetBrains.Annotations;

namespace SnakeHost.Logic
{
    public class Authenticator
    {
        public Authenticator(Settings settings)
        {
            _settings = settings;
        }

        public bool AuthorizePlayer([NotNull] Player player, string token)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            return player.Token == token;
        }

        public bool AuthorizeAdmin(string token)
        {
            return _settings.AdminToken == token;
        }

        private readonly Settings _settings;
    }
}