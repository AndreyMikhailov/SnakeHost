using System.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace SnakeHost.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public GameController(Game game)
        {
            _game = game;
        }

        [HttpPut]
        public void Start()
        {
            _game.Start();
        }
        
        [HttpPut]
        public void Stop()
        {
            _game.Stop();
        }
        
        [HttpPut]
        public void Pause()
        {
            _game.Pause();
        }
        
        [HttpPut]
        public void Resume()
        {
            _game.Resume();
        }

        [HttpPut]
        public void GameBoardSize(int width, int height)
        {
            _game.GameBoardSize = new Size(width, height);
        }

        [HttpPut]
        public void MaxFood(int count)
        {
            _game.MaxFood = count;
        }

        private readonly Game _game;
    }
}