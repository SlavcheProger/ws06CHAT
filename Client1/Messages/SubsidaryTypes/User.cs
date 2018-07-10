namespace Client1
{
    public class User
    {
        public string AccessToken { get; set; }
        public string Name { get; set; }

        public User() : this(string.Empty) { }
        public User(string accessToken)
        {
            AccessToken = accessToken;
            Name = string.Empty;
        }
    }
}
