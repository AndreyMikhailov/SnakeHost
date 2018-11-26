using System.Drawing;

namespace SnakeHost.Messages
{
    public class GameBoardSizeRequest : AuthenticationRequest
    {
        public Size Size { get; set; }
    }
}