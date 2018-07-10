using Newtonsoft.Json.Linq;

namespace Client1
{
    public class CommandMessage : BaseMessage
    {
        public JObject Args { set; get; }
        public CommandType CommandType { set; get; }

        public CommandMessage()
        {
            MessageType = MessageType.CommandMessage;
            Args = default(JObject);
        }
        

    }
}
