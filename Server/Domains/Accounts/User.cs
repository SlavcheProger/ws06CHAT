using System.ComponentModel.DataAnnotations;
using System.Linq;
using Chat.Socket.Server.Services;
using Chat.Socket.Server.Services.Interfaces;
using Newtonsoft.Json;

namespace Chat.Socket.Server.Domains.Accounts
{
    public class User
    {
        [JsonProperty("ID")]
        [Key]
        public int ID { get; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string AccessToken { get; }
		private readonly ISender sender;

        public User() : this(0, string.Empty, string.Empty) { }
        public User(int id, string accessToken, string name = null)
        {
            ID = id;
            Name = name ?? "Anonymous";
            Status = Status.Online;
            AccessToken = accessToken;
			sender = DependencyResolver.Get<ISender>();
        }

        public void UpdateName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
				string oldName;
                Name = name;
                using (var context = new DataBaseContext())
                {
                    var user = context.Users.FirstOrDefault(c => c.AccessToken == AccessToken);
					oldName = user.Name;
                    user.Name = name;
                    context.SaveChanges();
                }
                sender.ServerNotificationToAll($"{oldName} changed name to {name}");
			}
		}
    }
}
