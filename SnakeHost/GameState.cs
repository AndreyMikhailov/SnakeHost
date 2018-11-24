using System.Drawing;

namespace SnakeHost
{
    public class GameState
    {
        public bool IsStarted { get; set; }
        public bool IsPaused { get; set; }
        public Size GameBoardSize { get; set; }
        public int MaxFood { get; set; }
        public PlayerState[] Players { get; set; }
        public Point[] Food { get; set; }
    }
}