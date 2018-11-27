namespace SnakeHost.Messages
{
    public class GameSettingsRequest : AuthenticationRequest
    {
        public GameSettingsState Settings { get; set; }
    }
}