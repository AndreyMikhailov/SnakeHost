﻿using System.Drawing;
using SnakeHost.Logic;

namespace SnakeHost.Messages
{
    public class GameSettingsState
    {
        public Size GameBoardSize { get; set; }
        public CrashRule CrashRule { get; set; }
        public bool AutoRestart { get; set; }
        public int MaxFood { get; set; }
        public int MaxWalls { get; set; }
        public Size MaxWallSize { get; set; }
        public Size MinWallSize { get; set; }
        public double TurnTimeSeconds { get; set; }
        public double RoundTimeSeconds { get; set; }
    }
}