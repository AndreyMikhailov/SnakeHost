namespace SnakeHost.Messages
{
    public class DeletePlayerRequest : AuthenticationRequest
    {
        public string PlayerName { get; set; }
    }
}