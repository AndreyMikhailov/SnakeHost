using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using SnakeHost.Logic;
using SnakeHost.Messages;

namespace SnakeHost.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public GameController(Authenticator authenticator, Game game)
        {
            _authenticator = authenticator;
            _game = game;
        }

        [HttpPost]
        [ActionName("player")]
        public RegisterPlayerReply RegisterPlayer(RegisterPlayerRequest request)
        {
            if (!IsAdmin(request))
            {
                return null;
            }

            var player = _game.RegisterPlayer(request.PlayerName);
            return new RegisterPlayerReply
            {
                Password = player.Password
            };
        }
        
        [HttpDelete]
        [ActionName("player")]
        public void DeletePlayer(DeletePlayerRequest request)
        {
            if (IsAdmin(request))
            {
                _game.DeletePlayer(request.PlayerName);
            }
        }

        [HttpPost]
        [ActionName("start")]
        public void Start(AuthenticationRequest request)
        {
            if (IsAdmin(request))
            {
                _game.Start();
            }
        }
        
        [HttpPost]
        [ActionName("stop")]
        public void Stop(AuthenticationRequest request)
        {
            if (IsAdmin(request))
            {
                _game.Stop();
            }
        }
        
        [HttpPost]
        [ActionName("pause")]
        public void Pause(AuthenticationRequest request)
        {
            if (IsAdmin(request))
            {
                _game.Pause();
            }
        }
        
        [HttpPost]
        [ActionName("resume")]
        public void Resume(AuthenticationRequest request)
        {
            if (IsAdmin(request))
            {
                _game.Resume();
            }
        }

        [HttpPost]
        [ActionName("gameboardsize")]
        public void SetGameBoardSize(GameBoardSizeRequest request)
        {
            if (IsAdmin(request))
            {
                _game.GameBoardSize = request.Size;
            }
        }

        [HttpPost]
        [ActionName("maxfood")]
        public void SetMaxFood(MaxFoodRequest request)
        {
            if (IsAdmin(request))
            {
                _game.MaxFood = request.MaxFood;
            }
        }

        [HttpPost]
        [ActionName("autorestart")]
        public void SetAutoRestart(AutoRestartRequest request)
        {
            if (IsAdmin(request))
            {
                _game.AutoRestart = request.AutoRestart;
            }
        }

        private bool IsAdmin([NotNull] AuthenticationRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return request.Credentials?.IsValid() == true &&
                   _authenticator.CheckAdmin(request.Credentials);
        }

        private readonly Authenticator _authenticator;
        private readonly Game _game;
    }
}