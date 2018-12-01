using System.Drawing;

namespace SnakeHost.Messages
{
    public class PlayerState
    {
        public string Name { get; set; }
        public bool IsSpawnProtected { get; set; }
        public Point[] Snake { get; set; }
    }
}