namespace SnakeHost.Messages
{
    public class AutoRestartRequest : AuthenticationRequest
    {
        public bool AutoRestart { get;set; }
    }
}