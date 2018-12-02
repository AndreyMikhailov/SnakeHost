// ReSharper disable CommentTypo
namespace SnakeHost.Messages
{
    public class AuthenticationRequest
    {
        /// <summary>Уникальный секретный токен для авторизации игрока.</summary>
        public string Token { get; set; }
    }
}