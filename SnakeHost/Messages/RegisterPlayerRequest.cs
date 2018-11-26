namespace SnakeHost.Messages
{
    public class RegisterPlayerRequest : AuthenticationRequest
    {
        public string PlayerName { get; set; }
    }
}