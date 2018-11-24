using Microsoft.AspNetCore.Mvc;

namespace SnakeHost.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        public PlayerController(Game game)
        {
            _game = game;
        }

        [HttpPost]
        public void Register(string name)
        {
            _game.RegisterPlayer(name);
        }

        [HttpGet]
        public GameState GameBoard()
        {
            return _game.GetState();
        }

        [HttpPut]
        public void Direction(string player, Direction direction)
        {
            _game.SetPlayerDirection(player, direction);
        }

        private readonly Game _game;
    }
}