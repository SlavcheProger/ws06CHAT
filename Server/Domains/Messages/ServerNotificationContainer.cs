using Newtonsoft.Json;

namespace Chat.Socket.Server.Domains.Messages
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
