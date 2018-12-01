using Microsoft.AspNetCore.Http;
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
        
        [HttpGet]
        [ActionName("name")]
        public NameResponse GetName(AuthenticationRequest request)
        {
            if (!TryGetPlayerAndAuthorize(request, out var player))
            {
                return null;
            }
            return new NameResponse { Name = player.Name };
        }

        [HttpPost]
        [ActionName("direction")]
        public void SetDirection(DirectionRequest request)
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