using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SnakeHost.Helpers;

namespace SnakeHost.Logic
{
    public class Player
    {
        public Player([NotNull] string name)
            : this(name, PasswordGenerator.Generate(20, 0))
        {
        }

        [JsonConstructor]
        public Player([NotNull] string name, [NotNull] string token)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Token = token;
        }

        public string Name { get; }

        public string Token { get; }
    }
}