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
        /// <returns>Состояние игрового поля.</returns>
        [HttpGet]
        [ActionName("gameboard")]
        public GameStateResponse GetGameBoard()
        {
            return _game.GetState();
        }
        
        /// <summary>Возвращает имя игрока.</summary>
        /// <param name="request">Данные для авторизации.</param>
        /// <returns>Имя игрока.</returns>
        [HttpGet]
        [ActionName("name")]
        public NameResponse GetName([FromBody] AuthenticationRequest request)
        {
            if (!TryGetPlayerAndAuthorize(request, out var player))
            {
                return null;
            }
            return new NameResponse { Name = player.Name };
        }

        /// <summary>Задает поворот змейки, который будет использван во время следующего хода.</summary>
        /// <param name="request">Данные авторизации и направление змейки.</param>
        [HttpPost]
        [ActionName("direction")]
        public void SetDirection([FromBody] DirectionRequest request)
        {
            if (TryGetPlayerAndAuthorize(request, out var player))
            {
                _game.SetPlayerDirection(player, request.Direction);
            }
        }

        private bool TryGetPlayerAndAuthorize(AuthenticationRequest request, out Player player)
        {
            if (_game.TryFindPlayerByToken(request.Token, out player) && 
                _authenticator.AuthorizePlayer(player, request.Token))
            {
                return true;
            }
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }

        private readonly Authenticator _authenticator;
        private readonly Game _game;
    }
}