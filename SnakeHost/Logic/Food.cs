﻿using System.Drawing;

namespace SnakeHost.Logic
{
    public class Food
    {
        public Food(Point position)
        {
            Position = position;
        }

        public Point Position { get; }

        public bool IsEaten { get; private set; }

        public void Eat()
        {
            IsEaten = true;
        }
    }
}