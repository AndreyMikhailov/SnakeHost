using System;
using JetBrains.Annotations;

namespace SnakeHost
{
    public class Player
    {
        public Player([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        [CanBeNull]
        public Snake Snake { get; set; }
    }
}