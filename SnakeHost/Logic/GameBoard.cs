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

        public CrashRule CrashRule { get; set; }

        public int MaxFood { get; set; } = 20;

        public int MaxWalls { get; set; } = 20;

        public Size MaxWallSize { get; set; } = new Size(10, 10);

        public Size MinWallSize { get; set; } = new Size(3, 3);

        public int SpawnProtectionTurns { get; set; } = 4;

        public int TurnNumber { get; private set; }

        public void SetPlayerDirection(Player player, Direction direction)
        {
            if (!_playersData.TryGetValue(player, out var playerData))
            {
                return;
            }

            playerData.IsAfk = false;
            playerData.Snake?.SetDirection(direction);
        }

        public void NextTurn()
        {
            if (TurnNumber == 0)
            {
                GenerateWalls();
                GenerateFood();
                RespawnDead();
            }
            else
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

            TurnNumber++;
        }
        
        public GameStateResponse GetState()
        {
            return new GameStateResponse
            {
                TurnNumber = TurnNumber,
                GameBoardSize = Size,
                MaxFood = MaxFood,
                Food = _foodList.Select(f => f.Position).ToArray(),
                Walls = _walls.Select(w => w.Rectangle).ToArray(),
                Players = _playersData.Select(playerData => 
                    new PlayerState
                    {
                        Name = playerData.Player.Name,
                        IsSpawnProtected = IsSpawnProtected(playerData),
                        Snake = playerData.Snake?.Body.ToArray()
                    }).ToArray()
            };
        }

        private void MoveAll()
        {
            foreach (var snake in GetNotProtectedSnakes())
            {
                snake.Move(_foodList);
            }
        }

        private void KillCrashedByHeads()
        {
            if (CrashRule == CrashRule.CrashedIntoSideDies)
            {
                foreach (var snake in GetNotProtectedSnakes())
                {
                    if (snake.IsCrashedIntoOthersHead(GetNotProtectedSnakes()))
                    {
                        snake.Kill();
                    }
                }
            }
        }

        private void KillCrashed()
        {
            foreach (var snake in GetNotProtectedSnakes())
            {
                if (!snake.IsInside(Size) || 
                    snake.IsCrashedIntoItself() ||
                    IsCrashedByRule(snake, GetNotProtectedSnakes()) || 
                    snake.IsCrashedIntoOther(GetProtectedSnakes()) ||
                    snake.IsCrashedIntoWall(_walls))
                {
                    snake.Kill();
                }
            }
        }

        private bool IsCrashedByRule(Snake snake, IEnumerable<Snake> others)
        {
            switch (CrashRule)
            {
                case CrashRule.ShortestDies:
                    var othersArray = others.ToArray();
                    return snake.IsCrashedIntoLongerOrEqual(othersArray) || snake.IsCrashedByLonger(othersArray);
                case CrashRule.CrashedIntoSideDies:
                    return snake.IsCrashedIntoOther(others);
                default:
                    return true;
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
                    playerData.DeathTurnNumber = TurnNumber;
                }
            }
        }

        private void RemoveEatenFood()
        {
            _foodList.RemoveAll(food => food.IsEaten);
        }

        private void RespawnDead()
        {
            var deadPlayers = _playersData.Where(IsDead);

            foreach (var playerData in deadPlayers)
            {
                if (!playerData.IsAfk && TryGenerateSnakeOnFreeSpace(out var snake))
                {
                    playerData.Snake = snake;
                }
            }
        }

        private void GenerateWalls()
        {
            for (var i = 0; i < MaxWalls; i++)
            {
                if (TryGenerateWallOnBoard(out var wall))
                {
                    _walls.Add(wall);
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
                   _walls.All(w => !w.Intersects(point)) &&
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

        private bool TryGenerateWallOnBoard(out Wall wall)
        {
            for (var i = 0; i < GeneratorMaxTries; i++)
            {
                var point = GeneratePointOnBoard();
                var width = _random.Next(MinWallSize.Width, MaxWallSize.Width + 1);
                var height = _random.Next(MinWallSize.Height, MaxWallSize.Height + 1);
                var rectangle = new Rectangle(point.X, point.Y, width, height);

                if (rectangle.Right <= Size.Width && rectangle.Bottom <= Size.Height)
                {
                    wall = new Wall(rectangle);
                    return true;
                }
            }
            wall = null;
            return false;
        }

        [ItemNotNull]
        private IEnumerable<Snake> GetAllSnakes()
        {
            return _playersData
                .Select(playerData => playerData.Snake)
                .Where(snake => snake != null);
        }

        [ItemNotNull]
        private IEnumerable<Snake> GetNotProtectedSnakes()
        {
            return _playersData
                .Where(playerData => !IsSpawnProtected(playerData))
                .Select(playerData => playerData.Snake)
                .Where(snake => snake != null);
        }

        [ItemNotNull]
        private IEnumerable<Snake> GetProtectedSnakes()
        {
            return _playersData
                .Where(IsSpawnProtected)
                .Select(playerData => playerData.Snake)
                .Where(snake => snake != null);
        }

        private bool IsSpawnProtected(PlayerData playerData)
        {
            return !IsDead(playerData) && 
                   !playerData.IsAfk &&
                   playerData.DeathTurnNumber.HasValue && 
                   TurnNumber - playerData.DeathTurnNumber < SpawnProtectionTurns;
        }

        private static bool IsDead(PlayerData playerData)
        {
            return playerData.Snake?.IsAlive != true;
        }

        private readonly PlayerDataCollection _playersData;
        private readonly List<Food> _foodList = new List<Food>();
        private readonly List<Wall> _walls = new List<Wall>();
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

            public bool IsAfk { get; set; } = true;

            public int? DeathTurnNumber { get; set; }
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