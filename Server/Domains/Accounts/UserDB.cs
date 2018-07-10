using System.ComponentModel.DataAnnotations;

namespace Chat.Socket.Server.Domains.Accounts
{
    class UserDB
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string AccessToken { get; set; }

        public UserDB() : this("Anonymous", string.Empty) { }
        public UserDB(string name, string accessToken)
        {
            Name = name ?? "Anonymous";
            AccessToken = accessToken;
            Status = Status.Online;
        }
    }
}
