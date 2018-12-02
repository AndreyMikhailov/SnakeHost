using System.Drawing;

namespace SnakeHost.Messages
{
    public class GameStateResponse
    {
        public bool IsStarted { get; set; }
        public bool IsPaused { get; set; }
        public int RoundNumber { get; set; }
        public int TurnNumber { get; set; }
        public double TimeUntilNextTurnSeconds { get; set; }
        public Size GameBoardSize { get; set; }
        public int MaxFood { get; set; }
        public PlayerState[] Players { get; set; }
        public Point[] Food { get; set; }
        public Rectangle[] Walls { get; set; }
    }
}