using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat.Socket.Server.Domains.Accounts;
using Chat.Socket.Server.Domains.Messages;
using Chat.Socket.Server.Services;
using Chat.Socket.Server.Services.Interfaces;
using Newtonsoft.Json;

namespace Chat.Socket.Server
{
	public class ClientConnection
	{
		// Data
		private readonly TcpClient client;
		internal User UserObject { get; set; }
		private bool pingAnswered;
		internal NetworkStream Stream { get; }
		private Thread thread;

		// Services
		private readonly ISender sender;
		private readonly ICommandHandler commandHandler;
		private readonly IUsersService usersService;
		private readonly ILogger logger;
		private readonly IPingIdGenerator pingIdGenerator;
		private readonly IConnectionsService connectionsService;

		private readonly System.Timers.Timer timer;

		public ClientConnection(TcpClient tcpClient)
		{
			client = tcpClient;

			sender = DependencyResolver.Get<ISender>();
			commandHandler = DependencyResolver.Get<ICommandHandler>();
			usersService = DependencyResolver.Get<IUsersService>();
			logger = DependencyResolver.Get<ILogger>();
			pingIdGenerator = DependencyResolver.Get<IPingIdGenerator>();
			connectionsService = DependencyResolver.Get<IConnectionsService>();

			try
			{
				Stream = client.GetStream();
			}
			catch (Exception exception)
			{
				logger.Write(exception);
			}

			UserObject = new User();
			pingAnswered = true;

			timer = new System.Timers.Timer(60 * 1000);
			timer.Elapsed += delegate { Ping(); };
			timer.AutoReset = true;
			timer.Start();
		}

		public void Launch()
		{
			thread = new Thread(() => GettingMessagesProcess());
			thread.Start();
		}

		public void GettingMessagesProcess()
		{
			while (true)
			{
				try
				{
					MessageHandler(GetMessage());
				}
				catch (Exception exception)
				{
					logger.Write(exception);
					connectionsService.Remove(this);
					break;
				}
			}
		}

		private string GetMessage()
		{
			var data = new byte[2097152];
			var builder = new StringBuilder();
			var bytes = 0;

			do
			{
				bytes = Stream.Read(data, 0, data.Length);
				builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
			}
			while (Stream.DataAvailable);

			var messageJsonString = builder.ToString();
			logger.Write(messageJsonString);

			return messageJsonString;
		}

		private void MessageHandler(string messageJsonString)
		{
			MessageType messageType;

			//try
			//{
			//	if (!string.IsNullOrEmpty(messageJsonString))
			//	{
			messageType = JsonConvert.DeserializeObject<BaseMessageContainer>(messageJsonString).Type;
			//	} 
			//	else
			//	{
			//		return;
			//	}
			//}
			//catch (Exception exception)
			//{
			//	logger.Write(exception);
			//	sender.ServerNotification(this, "Incorrect comand format.");
			//	return;
			//}

			if (messageType == MessageType.CommandMessage)
			{
				CommandSwitchHandler(messageJsonString);
			}

			else if (messageType == MessageType.ChatMessageFromClient && usersService.IsAuthorised(UserObject.AccessToken))
			{
				ChatMessageHandler(messageJsonString);
			}

			else sender.ServerNotification(this, "You are not authorized or incorrect message type.");
		}

		private void ChatMessageHandler(string chatMessageJsonString)
		{
			var chatMessageJson = JsonConvert.DeserializeObject<ChatMessageFromClientContainer>(chatMessageJsonString);
			sender.Broadcast(this, chatMessageJson.Text, chatMessageJson.Args);
		}

		private void CommandSwitchHandler(string commandJsonString)
		{
			var commandJson = JsonConvert.DeserializeObject<CommandMessageContainer>(commandJsonString);

			if (commandJson.Command == CommandType.Start && !usersService.IsAuthorised(UserObject.AccessToken))
			{
				commandHandler.Start(this, commandJson.Args["AccessToken"].ToString());
			}

			else if (usersService.IsAuthorised(UserObject.AccessToken))
			{
				switch (commandJson.Command)
				{
					case CommandType.SetUsername:
						commandHandler.SetUsername(this, commandJson.Args["Name"].ToString());
						break;

					case CommandType.Here:
						commandHandler.Here(this);
						break;

					case CommandType.Ping: // it's possible to encapsulate this into commandHandler.Pong
						try
						{
							sender.Pong(this, commandJson.Args["ID"].ToObject<int>());
						}
						catch
						{
							sender.ServerNotification(this, "Incorrect ID for command /Ping");
						}
						break;

					case CommandType.Pong:
						CheckPong(commandJson.Args["ID"].ToObject<int>());
						break;

					case CommandType.SendFile:
						sender.BroadcastFile(this, commandJson.Args);
						break;

					default:
						sender.ServerNotification(this, "Incorrect command.");
						break;
				}
			}
			else
			{
				sender.ServerNotification(this, "You are not authorized.");
			}
		}

		public void Ping()
		{
			if (pingAnswered == true)
			{
				pingAnswered = false;
				int ID = pingIdGenerator.NextPingIdGenerator;
				sender.Ping(this, ID);
			}
			else
			{
				connectionsService.Remove(this);
			}
		}

		private void CheckPong(int ID)
		{
			if (ID == pingIdGenerator.CurrentPingIdGenerator)
			{
				pingAnswered = true;
			}
		}

		public void Close()
		{
			logger.Write($"{UserObject.Name} left");
			sender.ServerNotificationToAll($"{UserObject.Name} left");

			timer.Stop();
			timer.Dispose();
			client?.Close();
			Stream?.Close();
			thread.Abort();
		}
	}
}