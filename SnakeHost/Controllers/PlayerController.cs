using Microsoft.AspNetCore.Mvc;
using SnakeHost.Logic;
using SnakeHost.Messages;

namespace SnakeHost.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        public PlayerController(Authenticator authenticator, Game game)
        {
            _authenticator = authenticator;
            _game = game;
        }
        
        [HttpGet]
        [ActionName("gameboard")]
        public GameStateResponse GetGameBoard()
        {
            return _game.GetState();
        }

        [HttpPost]
        [ActionName("direction")]
        public void SetDirection(DirectionRequest request)
        {
            var credentials = request.Credentials;
            if (credentials == null || !credentials.IsValid())
            {
                return;
            }

            if (_game.TryFindPlayer(credentials.Name, out var player) &&
                _authenticator.CheckPlayer(player, credentials))
            {
                _game.SetPlayerDirection(player, request.Direction);
            }
        }

        private readonly Authenticator _authenticator;
        private readonly Game _game;
    }
}