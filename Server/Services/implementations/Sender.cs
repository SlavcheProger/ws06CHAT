using Chat.Socket.Server.Services.Interfaces;
using System.Text;
using Chat.Socket.Server.Domains.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Chat.Socket.Server.Domains.Accounts;

namespace Chat.Socket.Server.Services.Implementations
{
    public class Sender : ISender
    {
        private readonly IConnectionsService connectionsService;
        private readonly ILogger logger;
        private readonly IDataStorage dataStorage;
        private readonly IUsersService usersService;

        private readonly ISender self;

        public Sender()
        {
            connectionsService = DependencyResolver.Get<IConnectionsService>();
            logger = DependencyResolver.Get<ILogger>();
            dataStorage = DependencyResolver.Get<IDataStorage>();
            usersService = DependencyResolver.Get<IUsersService>();

            self = this as ISender;
        }

        #region ISender
        bool ISender.RecipientIsValid(JObject Args)
        {
            var recipientName = Args["Recipient"].ToString();
			if (!string.IsNullOrEmpty(recipientName) && recipientName != "Anonymous")
			{
				using (var context = new DataBaseContext())
				{
					if (context.Users.Any(u => u.Name == recipientName))
					{
						return usersService.IsOnline(recipientName);

					}
				}
			}
            return false;
        }

        void ISender.Broadcast(ClientConnection connection, string messageText, JObject Args)
        {
            var connections = connectionsService.Connections;
            var transportContainer = new ChatMessageFromServerContainer(messageText);

            if (Args != null && Args.ContainsKey("Recipient"))
            {
                if (self.RecipientIsValid(Args) && connections.Any(u => u.UserObject.Name == Args["Recipient"].ToString()))
                {
                    transportContainer.Name = $"[@]{connection.UserObject.Name}";
                    self.MessageToRecipient(Args["Recipient"].ToString(), transportContainer);

                    var recipientId = connectionsService.Connections.FirstOrDefault(u => u.UserObject.Name == Args["Recipient"].ToString()).UserObject.ID;
                    dataStorage.SaveMessage(messageText, recipientId, connection.UserObject.ID);
                }
            }
            else
            {
                transportContainer.Name = connection.UserObject.Name;
                self.MessageToAll(connection, transportContainer);

                dataStorage.SaveMessage(messageText, 0, connection.UserObject.ID);
            }
        }

        void ISender.MessageToRecipient(string recipientName, ChatMessageFromServerContainer transportContainer)
        {
            var recipientConnections = connectionsService.Connections.Where(u => u.UserObject.Name == recipientName);

            var transportContainerJson = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerJson);

            foreach (var recipientConnection in recipientConnections)
            {
                recipientConnection.Stream.Write(byteString, 0, byteString.Length);
            }
        }

        void ISender.MessageToAll(ClientConnection connection, ChatMessageFromServerContainer transportContainer)
        {
            var connections = connectionsService.Connections;
            var transportContainerJson = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerJson);

            foreach (var connectionItem in connections)
            {
                if (connectionItem != connection)
                {
                    connectionItem.Stream.Write(byteString, 0, byteString.Length);
                }
            }
        }

        void ISender.BroadcastFile(ClientConnection connection, JObject Args)
        {
            var transportContainer = new CommandMessageContainer(CommandType.SendFile);

            if (Args != null && Args.ContainsKey("Extension") && Args.ContainsKey("File"))
            {
                transportContainer.Args["File"] = Args["File"].ToString();
                transportContainer.Args["Extension"] = Args["Extension"].ToString();
            }
            else
            {
                self.ServerNotification(connection, "Incorrect Args.");
                return;
            }

            if (Args.ContainsKey("Recipient"))
            {
                if (self.RecipientIsValid(Args))
                {
                    self.FileToRecipient(Args["Recipient"].ToString(), transportContainer);

                    var recipientId = connectionsService.Connections.FirstOrDefault(u => u.UserObject.Name == Args["Recipient"].ToString()).UserObject.ID;
                    dataStorage.SaveFile(connection.UserObject.ID, recipientId,
                        Convert.FromBase64String(Args["File"].ToString()),
                        Args["Extension"].ToString());
                }
            }
            else
            {
                self.FileToAll(connection, transportContainer);

                dataStorage.SaveFile(connection.UserObject.ID,
                    0, Convert.FromBase64String(Args["File"].ToString()),
                    Args["Extension"].ToString());
            }
        }

        void ISender.FileToRecipient(string recipientName, CommandMessageContainer transportContainer)
        {
            var recipientConnections = connectionsService.Connections
                .Where(u => u.UserObject.Name == recipientName);

            var transportContainerJson = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerJson);

            foreach (var recipientConnection in recipientConnections)
            {
                recipientConnection.Stream.Write(byteString, 0, byteString.Length);
            }

            logger.Write("File sended.");
        }

        void ISender.FileToAll(ClientConnection connection, CommandMessageContainer transportContainer)
        {
            var connections = connectionsService.Connections;
            var transportContainerJson = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerJson);

            foreach (var connectionItem in connections)
            {
                if (connectionItem != connection)
                {
                    connectionItem.Stream.Write(byteString, 0, byteString.Length);
                }
            }
            logger.Write("File sended.");
        }

        void ISender.ServerNotification(ClientConnection connection, string notificationText)
        {
            logger.Write("ServerNotification");
            var transportContainer = new ServerNotificationContainer(notificationText);

            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);
            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);
            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }

        void ISender.ServerNotificationToAll(string notificationText, ClientConnection connection = null)
        {
            var connections = connectionsService.Connections;

            foreach (var connectionItem in connections)
            {
                if (connectionItem != connection)
                {
                    self.ServerNotification(connectionItem, notificationText);
                }
            }
        }

        void ISender.StartCallback(ClientConnection connection)
        {
            var transportContainer = new CommandMessageContainer(CommandType.Start);

            transportContainer.Args["AccessToken"] = connection.UserObject.AccessToken;
            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);

            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);

            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }

        void ISender.HereCallback(ClientConnection connection, string[] usersNames)
        {
            var usersNamesString = string.Join(",", usersNames);
            logger.Write(usersNamesString);
            var transportContainer = new CommandMessageContainer(CommandType.Here);

            transportContainer.Args["Users"] = usersNamesString;

            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);

            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);
            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }

        void ISender.Pong(ClientConnection connection, int ID)
        {
            var transportContainer = new CommandMessageContainer(CommandType.Pong);

            transportContainer.Args["ID"] = ID;

            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);

            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);
            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }

        void ISender.Ping(ClientConnection connection, int ID)
        {
            var transportContainer = new CommandMessageContainer(CommandType.Ping);

            transportContainer.Args["ID"] = ID;
            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);

            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);
            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }

        void ISender.SetUsernameCallback(ClientConnection connection, string username)
        {
            var transportContainer = new CommandMessageContainer(CommandType.SetUsername);

            transportContainer.Args["Name"] = username;

            var transportContainerJsonString = JsonConvert.SerializeObject(transportContainer);

            var byteString = Encoding.UTF8.GetBytes(transportContainerJsonString);
            connection.Stream.Write(byteString, 0, byteString.Length);

            logger.Write(transportContainerJsonString);
        }
        #endregion
    }
}
