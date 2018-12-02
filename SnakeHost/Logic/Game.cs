using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SnakeHost.Messages;
using SnakeHost.Storage;

namespace SnakeHost.Logic
{
    public class Game
    {
        public TimeSpan TurnTime { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan RoundTime { get; set; } = TimeSpan.FromMinutes(5);

        public TimeSpan SpawnProtectionTime { get; set; } = TimeSpan.FromSeconds(4);

        public Size GameBoardSize { get; set; } = new Size(100, 100);

        public CrashRule CrashRule { get; set; }

        public bool AutoRestart { get; set; } = true;

        public int MaxFood { get; set; } = 50;

        public Size MaxWallSize { get; set; } = new Size(10, 10);

        public Size MinWallSize { get; set; } = new Size(3, 3);

        public int MaxWalls { get; set; } = 20;

        public bool IsStarted { get; private set; }

        public bool IsPaused { get; private set; }

        public IEnumerable<Player> Players => _players;

        public Game()
        {
            _players = LoadPlayers();
        }

        public void Start()
        {
            lock (_syncObject)
            {
                IsStarted = true;
                _roundNumber++;

                _gameBoard = new GameBoard(GameBoardSize, _players)
                {
                    SpawnProtectionTurns = (int)(SpawnProtectionTime.Ticks / TurnTime.Ticks),
                    CrashRule = CrashRule,
                    MaxFood = MaxFood,
                    MaxWalls = MaxWalls,
                    MaxWallSize = MaxWallSize,
                    MinWallSize = MinWallSize
                };
                _turnStopWatch.Restart();
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

                if (HasPlayerToken(player.Token))
                {
                    throw new ApplicationException("Failed to generate unique token.");
                }
                _players.Add(player);
                SavePlayers();
                return player;
            }
        }

        public void DeletePlayer(string name)
        {
            lock (_syncObject)
            {
                _players.RemoveAll(player => string.Equals(player.Name, name, StringComparison.OrdinalIgnoreCase));
                SavePlayers();
            }
        }

        public bool TryFindPlayerByName(string name, out Player player)
        {
            lock (_syncObject)
            {
                player = _players.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
                return player != null;
            }
        }

        public bool TryFindPlayerByToken(string token, out Player player)
        {
            lock (_syncObject)
            {
                player = _players.FirstOrDefault(p => p.Token == token);
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
                state.RoundNumber = _roundNumber;
                state.TimeUntilNextTurnSeconds = GetTimeUntilNextTurn().TotalSeconds;
                return state;
            }
        }

        private async Task GameLoop()
        {
            var roundTime = RoundTime;
            var turnTime = TurnTime;
            var elapsedTime = TimeSpan.Zero;
            
            while (IsStarted)
            {
                if (IsPaused)
                {
                    await Task.Delay(turnTime);
                    continue;
                }

                if (elapsedTime >= roundTime)
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
                        _turnStopWatch.Stop();
                        _gameBoard.NextTurn();
                        _turnStopWatch.Restart();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                await Task.Delay(turnTime);
                elapsedTime += turnTime;
            }
        }

        private void InternalStop(bool waitGameLoop)
        {
            IsStarted = false;

            if (waitGameLoop)
            {
                _gameLoopTask.Wait();
                _turnStopWatch.Reset();
            }
        }

        private TimeSpan GetTimeUntilNextTurn()
        {
            if (!IsStarted || IsPaused)
            {
                return TimeSpan.Zero;
            }

            var nextTurnTime = TurnTime - _turnStopWatch.Elapsed;
            return nextTurnTime < TimeSpan.Zero ? TimeSpan.Zero : nextTurnTime;
        }

        private bool HasPlayerName(string name)
        {
            return TryFindPlayerByName(name, out _);
        }

        private bool HasPlayerToken(string token)
        {
            return TryFindPlayerByToken(token, out _);
        }

        private void SavePlayers()
        {
            _playersStorage.Write(_players);
        }

        private List<Player> LoadPlayers()
        {
            return _playersStorage.Read();
        }

        private GameBoard _gameBoard;
        private Task _gameLoopTask;
        private int _roundNumber;

        private readonly object _syncObject = new object();
        private readonly List<Player> _players;
        private readonly JsonStorage<List<Player>> _playersStorage = new JsonStorage<List<Player>>("players.json");
        private readonly Stopwatch _turnStopWatch = new Stopwatch();

        private const int MaxNameLength = 50;
    }
}