using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;
using SnakeHost.Messages;

namespace SnakeHost.Logic
{
    public class GameBoard
    {
        public GameBoard(Size size, IEnumerable<Player> players)
        {
            _playersData = new PlayerDataCollection(players);
            Size = size;
        }

        public Size Size { get; }

        public int MaxFood { get; set; } = 20;

        public void SetPlayerDirection(Player player, Direction direction)
        {
            FindSnake(player)?.SetDirection(direction);
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
        
        public GameStateResponse GetState()
        {
            return new GameStateResponse
            {
                GameBoardSize = Size,
                MaxFood = MaxFood,
                Food = _foodList.Select(f => f.Position).ToArray(),
                Players = _playersData.Select(playerData => 
                    new PlayerState
                    {
                        Name = playerData.Player.Name,
                        Snake = playerData.Snake?.Body.ToArray()
                    }).ToArray()
            };
        }

        private void MoveAll()
        {
            foreach (var snake in GetAllSnakes())
            {
                snake.Move(_foodList);
            }
        }

        private void KillCrashedByHeads()
        {
            foreach (var snake in GetAllSnakes())
            {
                if (snake.IsCrashedIntoOthersHead(GetAllSnakes()))
                {
                    snake.Kill();
                }
            }
        }

        private void KillCrashed()
        {
            foreach (var snake in GetAllSnakes())
            {
                if (!snake.IsInside(Size) || snake.IsCrashedIntoItself() || snake.IsCrashedIntoOthers(GetAllSnakes()))
                {
                    snake.Kill();
                }
            }
        }

        private void RemoveDead()
        {
            foreach (var playerData in _playersData)
            {
                var snake = playerData.Snake;
                if (snake != null && !snake.IsAlive)
                {
                    playerData.Snake = null;
                }
            }
        }

        private void RemoveEatenFood()
        {
            _foodList.RemoveAll(food => food.IsEaten);
        }

        private void RespawnDead()
        {
            var deadPlayers = _playersData.Where(playerData => playerData.Snake?.IsAlive != true);

            foreach (var playerData in deadPlayers)
            {
                if (TryGenerateSnakeOnFreeSpace(out var snake))
                {
                    playerData.Snake = snake;
                }
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

        private bool IsPointOnFreeSpace(Point point)
        {
            return IsPointOnBoard(point) && 
                   GetAllSnakes().All(s => !s.Intersects(point)) && 
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
        private Snake FindSnake(Player player)
        {
            return _playersData.TryGetValue(player, out var data) ? data.Snake : null;
        }

        [ItemNotNull]
        private IEnumerable<Snake> GetAllSnakes()
        {
            return _playersData
                .Select(playerData => playerData.Snake)
                .Where(snake => snake != null);
        }

        private readonly PlayerDataCollection _playersData;
        private readonly List<Food> _foodList = new List<Food>();
        private readonly Random _random = new Random();

        private const int GeneratorMaxTries = 200;

        private class PlayerData
        {
            public PlayerData(Player player)
            {
                Player = player;
            }

            public Player Player { get; }

            [CanBeNull]
            public Snake Snake { get; set; }
        }

        private class PlayerDataCollection : KeyedCollection<Player, PlayerData>
        {
            public PlayerDataCollection(IEnumerable<Player> players)
            {
                foreach (var player in players)
                {
                    Add(new PlayerData(player));
                }
            }

            protected override Player GetKeyForItem(PlayerData item) => item.Player;
        }
    }
}