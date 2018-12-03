using System.Drawing;

namespace SnakeHost.Messages
{
    /// <summary>Описывает точку на двумерной области.</summary>
    public class Point2D
    {
        /// <summary>Координата X.</summary>
        public int X { get; set; }
        
        /// <summary>Координата Y.</summary>
        public int Y { get; set; }

        public static Point2D FromPoint(Point point)
        {
            return new Point2D
            {
                X = point.X,
                Y = point.Y
            };
        }
    }
}