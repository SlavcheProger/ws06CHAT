using Newtonsoft.Json.Linq;

namespace Client1
{
    public class ChatMessageFromServer : BaseMessage
    {
        public string Message { set; get; }

        public string Name { set; get; }

        public ChatMessageFromServer() { }
    }
}
