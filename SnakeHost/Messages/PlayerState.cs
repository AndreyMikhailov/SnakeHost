// ReSharper disable CommentTypo

namespace SnakeHost.Messages
{
    /// <summary>Содержит состояния игрока и его змейки.</summary>
    public class PlayerState
    {
        /// <summary>Имя игрока.</summary>
        public string Name { get; set; }

        /// <summary>
        /// Является ли игрок защищенным после появления на игровой доске.
        /// В это время все кто в него врежутся - умрут, а сам игрок не может двигаться какое-то время.
        /// </summary>
        public bool IsSpawnProtected { get; set; }

        /// <summary>Все точки, из которых состоит змейка (X, Y), начиная с головы и заканчивая хвостом.</summary>
        public Point2D[] Snake { get; set; }
    }
}