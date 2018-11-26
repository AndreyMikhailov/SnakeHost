using SnakeHost.Logic;

namespace SnakeHost.Messages
{
    public class DirectionRequest : AuthenticationRequest
    {
        public Direction Direction { get; set; }
    }
}