using Newtonsoft.Json.Linq;

namespace Chat.Socket.ClientV2.MessageTypes
{
    class ChatMessageFromClient : BaseMessage
    {
        public string Message { get; set; }
        public JObject Args { get; set; }

        public ChatMessageFromClient()
        {
            Args = new JObject();
        }
    }
}
