using System;
using JetBrains.Annotations;
using SnakeHost.Helpers;

namespace SnakeHost.Logic
{
    public class Player
    {
        public Player([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Password = PasswordGenerator.Generate(20, 0);
        }

        public string Name { get; }

        public string Password { get; }
    }
}