namespace SnakeHost.Messages
{
    public class Credentials
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password);
        }
    }
}