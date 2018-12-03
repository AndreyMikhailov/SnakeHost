using System.Drawing;

namespace SnakeHost.Messages
{
    /// <summary>Описывает размер двумерной области.</summary>
    public class Size2D
    {
        /// <summary>Ширина.</summary>
        public int Width { get; set; }
        
        /// <summary>Высота.</summary>
        public int Height { get; set; }

        public static Size2D FromSize(Size size)
        {
            return new Size2D
            {
                Width = size.Width,
                Height = size.Height
            };
        }
    }
}