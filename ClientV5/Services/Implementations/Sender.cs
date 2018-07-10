using System.Net.Sockets;
using System.Text;
using Chat.Socket.Server.Services;
using ClientV5.Domains.Messages;
using ClientV5.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClientV5.Services.Implementations
{
    public class Sender: ISender
    {
        private readonly IAccessTokenStorage accessTokenStorage;
        private NetworkStream stream;
        private readonly IPingIdGenerator pingIdGenerator;
		private readonly IDisconnector disconnector;

        public Sender()
        {
            accessTokenStorage = DependencyResolver.Get<IAccessTokenStorage>();
            pingIdGenerator = DependencyResolver.Get<IPingIdGenerator>();
			disconnector = DependencyResolver.Get<IDisconnector>();
        }

        void ISender.SetStream(NetworkStream clientStream)
        {
            stream = clientStream;
        }

        void ISender.AuthorizeRequest()
        {
            var transportContainer = new CommandMessageContainer(CommandType.Start);
            transportContainer.Args["AccessToken"] = accessTokenStorage.Get();

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }

        void ISender.Pong(JObject Args)
        {
            var transportContainer = new CommandMessageContainer(CommandType.Pong);
            transportContainer.Args = Args;

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }

        void ISender.SetUsername(string name)
        {
            var transportContainer = new CommandMessageContainer(CommandType.SetUsername);
            transportContainer.Args["Name"] = name;

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }

        void ISender.Here()
        {
            var transportContainer = new CommandMessageContainer(CommandType.Here);

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }

        void ISender.Ping()
        {
			if (pingIdGenerator.PingAnswered)
			{
				pingIdGenerator.PingAnswered = false;
				var transportContainer = new CommandMessageContainer(CommandType.Ping);
				transportContainer.Args["ID"] = pingIdGenerator.NextPingIdGenerator;

				var transportContainerString = JsonConvert.SerializeObject(transportContainer);
				var byteString = Encoding.UTF8.GetBytes(transportContainerString);
				stream.Write(byteString, 0, byteString.Length);
			}
			else
			{
				disconnector.Disconnect();
			}
        }

        void ISender.ChatMessage(string message, string name = null)
        {
            var transportContainer = new ChatMessageFromClientContainer();
            transportContainer.Text = message;
            transportContainer.Args["Recipient"] = name;

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }

        void ISender.SendFile(CommandMessageContainer transportContainer, string recipientName = null)
        {
            transportContainer.Args["Recipient"] = recipientName;

            var transportContainerString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerString);
            stream.Write(byteString, 0, byteString.Length);
        }
    }
}
