using System;
using JetBrains.Annotations;
using SnakeHost.Messages;

namespace SnakeHost.Logic
{
    public class Authenticator
    {
        public Authenticator(Settings settings)
        {
            _settings = settings;
        }

        public bool CheckPlayer([NotNull] Player player, [NotNull] Credentials credentials)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            
            return string.Equals(player.Name, credentials.Name, StringComparison.OrdinalIgnoreCase) &&
                   player.Password == credentials.Password;
        }

        public bool CheckAdmin([NotNull] Credentials credentials)
        {
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));
            
            return string.Equals(_settings.AdminName, credentials.Name, StringComparison.OrdinalIgnoreCase) &&
                   _settings.AdminPassword == credentials.Password;
        }

        private readonly Settings _settings;
    }
}