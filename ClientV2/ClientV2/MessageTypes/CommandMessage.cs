using Newtonsoft.Json.Linq;

namespace Chat.Socket.ClientV2.MessageTypes
{
    public class CommandMessage : BaseMessage
    {
        public JObject Args { get; set; }
        public CommandType CommandType { set; get; }

        public CommandMessage() : this(MessageType.CommandMessage, CommandType.Start)
        {
        }

        public CommandMessage(MessageType messageType, CommandType commandType)
        {
            MessageType = messageType;
            CommandType = commandType;
            Args = new JObject();
        }

    }
}
