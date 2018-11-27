using System.Drawing;

namespace SnakeHost.Messages
{
    public class GameSettingsState
    {
        public Size GameBoardSize { get; set; }
        public bool AutoRestart { get; set; }
        public int MaxFood { get; set; }
        public int MaxWalls { get; set; }
        public Size MaxWallSize { get; set; }
        public Size MinWallSize { get; set; }
        public int TurnTimeSeconds { get; set; }
        public int RoundTimeSeconds { get; set; }
    }
}