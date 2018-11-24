using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace SnakeHost
{
    public class Game
    {
        public TimeSpan TurnTime { get; set; } = TimeSpan.FromSeconds(1);

        public Size GameBoardSize { get; set; } = new Size(100, 100);

        public int MaxFood
        {
            get => _gameBoard.MaxFood;
            set => _gameBoard.MaxFood = value;
        }

        public bool IsStarted { get; private set; }

        public bool IsPaused { get; private set; }

        public void Start()
        {
            lock (_syncObject)
            {
                IsStarted = true;
                _gameBoard = new GameBoard(GameBoardSize);
                _gameLoopTask = Task.Run(GameLoop);
            }
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                IsStarted = false;
                _gameLoopTask.Wait();
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

        public void RegisterPlayer(string name)
        {
            lock (_syncObject)
            {
                if (IsStarted)
                {
                    _gameBoard.AddPlayer(name);
                }
            }
        }

        public void SetPlayerDirection(string name, Direction direction)
        {
            lock (_syncObject)
            {
                if (IsStarted)
                {
                    _gameBoard.SetPlayerDirection(name, direction);
                }
            }
        }

        public GameState GetState()
        {
            lock (_syncObject)
            {
                var state = _gameBoard?.GetState() ?? new GameState();
                state.IsStarted = IsStarted;
                state.IsPaused = IsPaused;
                return state;
            }
        }

        private async Task GameLoop()
        {
            while (IsStarted)
            {
                if (IsPaused)
                {
                    await Task.Delay(TurnTime);
                    continue;
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
            }
        }

        private GameBoard _gameBoard;
        private Task _gameLoopTask;

        private readonly object _syncObject = new object();
    }
}