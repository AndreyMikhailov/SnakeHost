using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SnakeHost.Messages;

namespace SnakeHost.Logic
{
    public class Game
    {
        public TimeSpan TurnTime { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan RoundTime { get; set; } = TimeSpan.FromMinutes(5);

        public Size GameBoardSize { get; set; } = new Size(100, 100);

        public bool AutoRestart { get; set; } = true;

        public int MaxFood { get; set; } = 50;

        public bool IsStarted { get; private set; }

        public bool IsPaused { get; private set; }

        public void Start()
        {
            lock (_syncObject)
            {
                IsStarted = true;
                _gameBoard = new GameBoard(GameBoardSize, _players) { MaxFood = MaxFood };
                _gameLoopTask = Task.Run(GameLoop);
            }
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                InternalStop(waitGameLoop: true);
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public Player RegisterPlayer([NotNull] string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Invalid name length.");
            }

            lock (_syncObject)
            {
                if (HasPlayerName(name))
                {
                    throw new ArgumentException("Name already registered.", nameof(name));
                }

                var player = new Player(name);
                _players.Add(player);
                return player;
            }
        }

        public void DeletePlayer(string name)
        {
            lock (_syncObject)
            {
                _players.RemoveAll(player => string.Equals(player.Name, name, StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool TryFindPlayer(string name, out Player player)
        {
            lock (_syncObject)
            {
                player = _players.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
                return player != null;
            }
        }

        public void SetPlayerDirection([NotNull] Player player, Direction direction)
        {
            lock (_syncObject)
            {
                if (IsStarted)
                {
                    _gameBoard.SetPlayerDirection(player, direction);
                }
            }
        }

        public GameStateResponse GetState()
        {
            lock (_syncObject)
            {
                var state = _gameBoard?.GetState() ?? new GameStateResponse();
                state.IsStarted = IsStarted;
                state.IsPaused = IsPaused;
                return state;
            }
        }

        private async Task GameLoop()
        {
            var elapsedTime = TimeSpan.Zero;

            while (IsStarted)
            {
                if (IsPaused)
                {
                    await Task.Delay(TurnTime);
                    continue;
                }

                if (elapsedTime >= RoundTime)
                {
                    InternalStop(waitGameLoop: false);

                    if (AutoRestart)
                    {
                        Start();
                    }
                    return;
                }
                
                lock (_syncObject)
                {
                    try
                    {
                        _gameBoard.NextTurn();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                await Task.Delay(TurnTime);
                elapsedTime += TurnTime;
            }
        }

        private void InternalStop(bool waitGameLoop)
        {
            IsStarted = false;

            if (waitGameLoop)
            {
                _gameLoopTask.Wait();
            }
            WriteScores();
        }

        private void WriteScores()
        {
            // TODO
        }

        private bool HasPlayerName(string name)
        {
            return TryFindPlayer(name, out _);
        }

        private GameBoard _gameBoard;
        private Task _gameLoopTask;

        private readonly object _syncObject = new object();
        private readonly List<Player> _players = new List<Player>();

        private const int MaxNameLength = 50;
    }
}