using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chat.Socket.Server.Domains.Messages
{
    public class CommandMessageContainer: BaseMessageContainer
    {
        [JsonProperty("CommandType")]
        public CommandType Command { get; set; }
        public JObject Args { get; set; }

        public CommandMessageContainer(CommandType commandType)
        {
            Command = commandType;
            Args = new JObject();
            Type = MessageType.CommandMessage;
        }
    }
}
