using Newtonsoft.Json;

namespace ClientV5.Domains.Messages
{
    public class ServerNotificationContainer: BaseMessageContainer
    {
        [JsonProperty("Message")]
        public string Text { get; set; }

        public ServerNotificationContainer(string notificationText)
        {
            Text = notificationText;
            Type = MessageType.ServerNotification;
        }
    }
}
