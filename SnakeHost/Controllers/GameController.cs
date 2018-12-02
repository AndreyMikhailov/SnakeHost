using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeHost.Logic;
using SnakeHost.Messages;

namespace SnakeHost.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
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
            if (!AuthorizeAdmin(request))
            {
                return null;
            }

            var player = _game.RegisterPlayer(request.PlayerName);
            return new RegisterPlayerReply
            {
                Token = player.Token
            };
        }
        
        [HttpDelete]
        [ActionName("player")]
        public void DeletePlayer(DeletePlayerRequest request)
        {
            if (AuthorizeAdmin(request))
            {
                _game.DeletePlayer(request.PlayerName);
            }
        }

        [HttpGet]
        [ActionName("player")]
        public Player[] GetPlayers(AuthenticationRequest request)
        {
            return AuthorizeAdmin(request) ? _game.Players.ToArray() : null;
        }

        [HttpPost]
        [ActionName("start")]
        public void Start(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request))
            {
                _game.Start();
            }
        }
        
        [HttpPost]
        [ActionName("stop")]
        public void Stop(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request))
            {
                _game.Stop();
            }
        }
        
        [HttpPost]
        [ActionName("pause")]
        public void Pause(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request))
            {
                _game.Pause();
            }
        }
        
        [HttpPost]
        [ActionName("resume")]
        public void Resume(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request))
            {
                _game.Resume();
            }
        }
        
        [HttpPost]
        [ActionName("gamesettings")]
        public void SetGameSettings(GameSettingsRequest request)
        {
            var settings = request.Settings;

            if (!AuthorizeAdmin(request) || settings == null)
            {
                return;
            }

            _game.GameBoardSize = settings.GameBoardSize;
            _game.CrashRule = settings.CrashRule;
            _game.AutoRestart = settings.AutoRestart;
            _game.MaxWallSize = settings.MaxWallSize;
            _game.MinWallSize = settings.MinWallSize;
            _game.MaxWalls = settings.MaxWalls;
            _game.MaxFood = settings.MaxFood;
            _game.RoundTime = TimeSpan.FromSeconds(settings.RoundTimeSeconds);
            _game.TurnTime = TimeSpan.FromSeconds(settings.TurnTimeSeconds);
        }

        [HttpGet]
        [ActionName("gamesettings")]
        public GameSettingsState GetGameSettings(AuthenticationRequest request)
        {
            if (!AuthorizeAdmin(request))
            {
                return null;
            }

            return new GameSettingsState
            {
                GameBoardSize = _game.GameBoardSize,
                CrashRule = _game.CrashRule,
                AutoRestart = _game.AutoRestart,
                MaxWallSize = _game.MaxWallSize,
                MinWallSize = _game.MinWallSize,
                MaxWalls = _game.MaxWalls,
                MaxFood = _game.MaxFood,
                RoundTimeSeconds = (int)_game.RoundTime.TotalSeconds,
                TurnTimeSeconds = (int)_game.TurnTime.TotalSeconds,
            };
        }

        private bool AuthorizeAdmin(AuthenticationRequest request)
        {
            if (_authenticator.AuthorizeAdmin(request.Token))
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