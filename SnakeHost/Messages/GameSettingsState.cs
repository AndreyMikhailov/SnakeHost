using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SnakeHost.Logic;

namespace SnakeHost.Messages
{
    public class GameSettingsState
    {
        public Size2D GameBoardSize { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public CrashRule CrashRule { get; set; }
        public bool AutoRestart { get; set; }
        public int MaxFood { get; set; }
        public int MaxWalls { get; set; }
        public Size2D MaxWallSize { get; set; }
        public Size2D MinWallSize { get; set; }
        public double TurnTimeSeconds { get; set; }
        public double RoundTimeSeconds { get; set; }
    }
}