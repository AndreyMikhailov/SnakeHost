using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeHost.Logic;
using SnakeHost.Messages;
// ReSharper disable CommentTypo

namespace SnakeHost.Controllers
{
    /// <summary>Позволяет игроку получить информацию о состоянии игрового поля и управлять его змейкой.</summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        public PlayerController(Authenticator authenticator, Game game)
        {
            _authenticator = authenticator;
            _game = game;
        }
        
        /// <summary>Возвращает состояние игрового поля.</summary>
        /// <param name="token">Токен игрока.</param>
        /// <returns>Состояние игрового поля.</returns>
        [HttpGet]
        [ActionName("gameboard")]
        public GameStateResponse GetGameBoard(string token = null)
        {
            Player player = null;
            if (token != null)
            {
                TryGetPlayerAndAuthorize(token, out player);
            }
            return _game.GetState(player);
        }
        
        /// <summary>Возвращает имя игрока.</summary>
        /// <param name="token">Токен игрока.</param>
        /// <returns>Имя игрока.</returns>
        /// <response code="401">Некорректный токен.</response>
        [HttpGet]
        [ActionName("name")]
        public NameResponse GetName([FromQuery] string token)
        {
            if (!TryGetPlayerAndAuthorize(token, out var player))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }
            return new NameResponse { Name = player.Name };
        }

        /// <summary>Задает поворот змейки, который будет использван во время следующего хода.</summary>
        /// <param name="request">Данные авторизации и направление змейки.</param>
        /// <response code="401">Некорректный токен.</response>
        [HttpPost]
        [ActionName("direction")]
        public void SetDirection([FromBody] DirectionRequest request)
        {
            if (TryGetPlayerAndAuthorize(request.Token, out var player))
            {
                _game.SetPlayerDirection(player, request.Direction);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        private bool TryGetPlayerAndAuthorize(string token, out Player player)
        {
            if (_game.TryFindPlayerByToken(token, out player) &&
                _authenticator.AuthorizePlayer(player, token))
            {
                return true;
            }
            player = null;
            return false;
        }

        private readonly Authenticator _authenticator;
        private readonly Game _game;
    }
}