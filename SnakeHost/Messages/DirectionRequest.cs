using SnakeHost.Logic;
// ReSharper disable CommentTypo

namespace SnakeHost.Messages
{
    /// <summary>Содержит данные авторизации и направление змейки.</summary>
    public class DirectionRequest : AuthenticationRequest
    {
        /// <summary>Направление змейки.</summary>
        public Direction Direction { get; set; }
    }
}