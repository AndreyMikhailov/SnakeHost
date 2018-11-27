using System.Drawing;
using SnakeHost.Logic;

namespace SnakeHost.Messages
{
    public class GameStateResponse
    {
        public bool IsStarted { get; set; }
        public bool IsPaused { get; set; }
        public int TurnNumber { get; set; }
        public Size GameBoardSize { get; set; }
        public int MaxFood { get; set; }
        public PlayerState[] Players { get; set; }
        public Point[] Food { get; set; }
        public Wall[] Walls { get; set; }
    }
}