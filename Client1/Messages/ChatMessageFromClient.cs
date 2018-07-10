using Newtonsoft.Json.Linq;

namespace Client1
{
    public class ChatMessageFromClient : BaseMessage
    {
        public string Message { set; get; }
        
        public JObject Args { get; set; }

        public ChatMessageFromClient()
        {
            MessageType = MessageType.ChatMessageFromClient;
        }
    }
}
