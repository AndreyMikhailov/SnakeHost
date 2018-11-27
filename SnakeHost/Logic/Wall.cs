using System.Drawing;

namespace SnakeHost.Logic
{
    public class Wall
    {
        public Wall(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }

        public Rectangle Rectangle { get; }

        public bool Intersects(Point point)
        {
            return Rectangle.Contains(point);
        }
    }
}