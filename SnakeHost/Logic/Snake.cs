using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SnakeHost.Logic
{
    public class Snake
    {
        public Snake(Point head)
        {
            _body = CreateDefaultBody(head);
        }

        public bool IsAlive { get; private set; } = true;

        public Point Head => _body.First.Value;

        /// <summary>All body points starting from head.</summary>
        public IReadOnlyCollection<Point> Body => _body;

        public void SetDirection(Direction direction)
        {
            if (!Enum.IsDefined(typeof(Direction), direction))
            {
                return;
            }

            if (!IsOppositeDirection(direction))
            {
                _nextDirection = direction;
            }
        }

        public void Move(IEnumerable<Food> foodList)
        {
            var newHead = GetNextHeadPosition();
            _body.AddFirst(newHead);

            var food = foodList.FirstOrDefault(f => f.Position == newHead);
            if (food != null)
            {
                food.Eat();
            }
            else
            {
                _body.RemoveLast();
            }

            _previousDirection = _nextDirection;
        }

        public bool IsCrashedIntoOther(IEnumerable<Snake> others)
        {
            return others
                .Where(other => other != this)
                .Any(other => other.Intersects(Head));
        }

        public bool IsCrashedIntoOthersHead(IEnumerable<Snake> others)
        {
            return others
                .Where(other => other != this)
                .Any(other => other.Head == Head);
        }

        public bool IsCrashedIntoLongerOrEqual(IEnumerable<Snake> others)
        {
            return others
                .Where(other => other != this)
                .Where(other => other.Body.Count >= Body.Count)
                .Any(other => other.Intersects(Head));
        }

        public bool IsCrashedByLonger(IEnumerable<Snake> others)
        {
            return others
                .Where(other => other != this)
                .Where(other => other.Body.Count > Body.Count)
                .Any(other => Intersects(other.Head));
        }

        public bool IsCrashedIntoWall(IEnumerable<Wall> walls)
        {
            return walls.Any(wall => wall.Intersects(Head));
        }

        public bool IsCrashedIntoItself()
        {
            return _body.Count > _body.Distinct().Count();
        }

        public bool IntersectsOther(IEnumerable<Snake> others)
        {
            var othersPoints = others.SelectMany(s => s.Body).Distinct();
            return Body.Except(othersPoints).Count() < Body.Count;
        }

        public void Kill()
        {
            IsAlive = false;
        }

        public bool Intersects(Point point)
        {
            return _body.Contains(point);
        }

        public bool IsInside(Size size)
        {
            return _body.All(point => IsPointInside(point, size));
        }

        private Point GetNextHeadPosition()
        {
            switch (_nextDirection)
            {
                case Direction.Left:
                    return new Point(Head.X - 1, Head.Y);
                case Direction.Top:
                    return new Point(Head.X, Head.Y - 1);
                case Direction.Right:
                    return new Point(Head.X + 1, Head.Y);
                case Direction.Bottom:
                    return new Point(Head.X, Head.Y + 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsOppositeDirection(Direction direction)
        {
            return (direction == Direction.Bottom && _previousDirection == Direction.Top) ||
                   (direction == Direction.Top && _previousDirection == Direction.Bottom) ||
                   (direction == Direction.Left && _previousDirection == Direction.Right) ||
                   (direction == Direction.Right && _previousDirection == Direction.Left);
        }

        private static LinkedList<Point> CreateDefaultBody(Point head)
        {
            return new LinkedList<Point>(new[]
            {
                head,
                new Point(head.X, head.Y + 1), 
            });
        }

        private static bool IsPointInside(Point point, Size size)
        {
            return point.X >= 0 && point.X < size.Width && point.Y >= 0 && point.Y < size.Height;
        }

        private Direction _nextDirection = Direction.Top;
        private Direction _previousDirection = Direction.Top;

        private readonly LinkedList<Point> _body;
    }
}
