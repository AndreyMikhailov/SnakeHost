using System.Drawing;

namespace SnakeHost.Messages
{
    /// <summary>Описывает прямоугольную область.</summary>
    public class Rectangle2D
    {
        /// <summary>Координата X верхнего левого угла области.</summary>
        public int X { get; set; }
        
        /// <summary>Координата Y верхнего левого угла области.</summary>
        public int Y { get; set; }

        /// <summary>Ширина области.</summary>
        public int Width { get; set; }
        
        /// <summary>Высота области.</summary>
        public int Height { get; set; }

        public static Rectangle2D FromRectangle(Rectangle rectangle)
        {
            return new Rectangle2D
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }
    }
}