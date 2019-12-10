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
            if (!AuthorizeAdmin(request.Token))
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
            if (AuthorizeAdmin(request.Token))
            {
                _game.DeletePlayer(request.PlayerName);
            }
        }

        [HttpGet]
        [ActionName("player")]
        public Player[] GetPlayers(string token)
        {
            return AuthorizeAdmin(token) ? _game.Players.ToArray() : null;
        }

        [HttpPost]
        [ActionName("start")]
        public void Start(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request.Token))
            {
                _game.Start();
            }
        }
        
        [HttpPost]
        [ActionName("stop")]
        public void Stop(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request.Token))
            {
                _game.Stop();
            }
        }
        
        [HttpPost]
        [ActionName("pause")]
        public void Pause(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request.Token))
            {
                _game.Pause();
            }
        }
        
        [HttpPost]
        [ActionName("resume")]
        public void Resume(AuthenticationRequest request)
        {
            if (AuthorizeAdmin(request.Token))
            {
                _game.Resume();
            }
        }
        
        [HttpPost]
        [ActionName("gamesettings")]
        public void SetGameSettings(GameSettingsRequest request)
        {
            var settings = request.Settings;

            if (!AuthorizeAdmin(request.Token) || settings == null)
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
        public GameSettingsState GetGameSettings(string token)
        {
            if (!AuthorizeAdmin(token))
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

        private bool AuthorizeAdmin(string token)
        {
            if (_authenticator.AuthorizeAdmin(token))
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