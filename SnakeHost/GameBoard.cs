using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;

namespace SnakeHost
{
    public class GameBoard
    {
        public GameBoard(Size size)
        {
            Size = size;
        }

        public Size Size { get; }

        public int MaxFood { get; set; } = 20;

        public IReadOnlyCollection<Player> Players => _players;

        public IReadOnlyCollection<Food> Food => _foodList;

        public void AddPlayer(string name)
        {
            if (FindPlayer(name) == null)
            {
                _players.Add(new Player(name));
            }
        }

        public void SetPlayerDirection(string name, Direction direction)
        {
            FindPlayer(name)?.Snake?.SetDirection(direction);
        }

        public void NextTurn()
        {
            MoveAll();
            KillCrashedByHeads();
            RemoveDead();
            KillCrashed();
            RemoveDead();
            RemoveEatenFood();
            RespawnDead();
            GenerateFood();
        }
        
        public GameState GetState()
        {
            return new GameState
            {
                GameBoardSize = Size,
                MaxFood = MaxFood,
                Food = Food.Select(f => f.Position).ToArray(),
                Players = Players.Select(p => new PlayerState
                {
                    Name = p.Name,
                    Snake = p.Snake?.Body.ToArray()
                }).ToArray()
            };
        }

        private void MoveAll()
        {
            foreach (var snake in _snakes)
            {
                snake.Move(_foodList);
            }
        }

        private void KillCrashedByHeads()
        {
            foreach (var snake in _snakes)
            {
                if (snake.IsCrashedIntoOthersHead(_snakes))
                {
                    snake.Kill();
                }
            }
        }

        private void KillCrashed()
        {
            foreach (var snake in _snakes)
            {
                if (!snake.IsInside(Size) || snake.IsCrashedIntoItself() || snake.IsCrashedIntoOthers(_snakes))
                {
                    snake.Kill();
                }
            }
        }

        private void RemoveDead()
        {
            _snakes.RemoveAll(snake => !snake.IsAlive);
        }

        private void RemoveEatenFood()
        {
            _foodList.RemoveAll(food => food.IsEaten);
        }

        private void RespawnDead()
        {
            var deadPlayers = _players.Where(p => p.Snake?.IsAlive != true);

            foreach (var player in deadPlayers)
            {
                TrySpawnSnake(out var snake);
                player.Snake = snake;
            }
        }

        private void GenerateFood()
        {
            var newFoodCount = MaxFood - _foodList.Count;

            for (var i = 0; i < newFoodCount; i++)
            {
                if (TryGeneratePointOnFreeSpace(out var point))
                {
                    _foodList.Add(new Food(point));
                }
            }
        }

        private void TrySpawnSnake(out Snake snake)
        {
            if (TryGenerateSnakeOnFreeSpace(out snake))
            {
                _snakes.Add(snake);
            }
        }

        private bool IsPointOnFreeSpace(Point point)
        {
            return IsPointOnBoard(point) && 
                   _snakes.All(s => !s.Intersects(point)) && 
                   _foodList.All(f => f.Position != point);
        }
        
        private bool ArePointsOnFreeSpace(IEnumerable<Point> points)
        {
            return points.All(IsPointOnFreeSpace);
        }
        
        private bool IsPointOnBoard(Point point)
        {
            return point.X >= 0 && point.X < Size.Width && point.Y >= 0 && point.Y < Size.Height;
        }

        private bool TryGeneratePointOnFreeSpace(out Point point)
        {
            for (var i = 0; i < GeneratorMaxTries; i++)
            {
                var generatedPoint = GeneratePointOnBoard();
                if (IsPointOnFreeSpace(generatedPoint))
                {
                    point = generatedPoint;
                    return true;
                }
            }
            point = Point.Empty;
            return false;
        }

        private bool TryGenerateSnakeOnFreeSpace(out Snake snake)
        {
            for (var i = 0; i < GeneratorMaxTries; i++)
            {
                if (!TryGeneratePointOnFreeSpace(out var head))
                {
                    snake = null;
                    return false;
                }

                var generatedSnake = new Snake(head);
                if (ArePointsOnFreeSpace(generatedSnake.Body))
                {
                    snake = generatedSnake;
                    return true;
                }
            }
            snake = null;
            return false;
        }

        private Point GeneratePointOnBoard()
        {
            var x = _random.Next(0, Size.Width + 1);
            var y = _random.Next(0, Size.Height + 1);
            return new Point(x, y);
        }

        [CanBeNull]
        private Player FindPlayer(string name)
        {
            return _players.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private readonly List<Snake> _snakes = new List<Snake>();
        private readonly List<Player> _players = new List<Player>();
        private readonly List<Food> _foodList = new List<Food>();
        private readonly Random _random = new Random();

        private const int GeneratorMaxTries = 100;
    }
}