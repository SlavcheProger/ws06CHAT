using Chat.Socket.ClientV2.Services.Interfaces;
using System.Text;
using System.Threading;
using Chat.Socket.ClientV2.Source;
using Newtonsoft.Json;
using Chat.Socket.ClientV2.MessageTypes;
using System;
using System.IO;

namespace Chat.Socket.ClientV2.Services.Implementations
{
    class Receiver : IReceiver
    {

        public Thread Thread { get; set; }

        private const string filePath = "../../ClientFiles/";
        private IReceiver self;

        private readonly ISender sender;
        private readonly IAutoPinger autoPinger;

        public Receiver()
        {
            autoPinger = DependencyResolver.Get<IAutoPinger>();
            sender = DependencyResolver.Get<ISender>();
            self = this as IReceiver;
        }

        void IReceiver.ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    var data = new byte[2097152];
                    var builder = new StringBuilder();
                    var bytes = 0;

                    do
                    {
                        bytes = ServerConnection.Stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (ServerConnection.Stream.DataAvailable);

                    var message = builder.ToString();

                    self.Sorter(message);
                }
                catch(Exception ex)
                {
                     Console.WriteLine(ex); 
                }
            }
        }


        void IReceiver.Sorter(string message)
        {
            var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(message);

            switch (baseMessage.MessageType)
            {
                case MessageType.CommandMessage:
                    self.CommandSorter(message);
                    break;

                case MessageType.ServerNotification:
                    self.NotificationHandler(message);
                    break;

                case MessageType.ChatMessageFromServer:
                    self.ChatMessageHandler(message);
                    break;
            }
        }

        void IReceiver.NotificationHandler(string message)
        {
            var serverNotification = JsonConvert.DeserializeObject<ServerNotification>(message);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Server notification: {0}", serverNotification.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        void IReceiver.ChatMessageHandler(string message)
        {
            var chatMessageFromServer = JsonConvert.DeserializeObject<ChatMessageFromServer>(message);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0}: {1}", chatMessageFromServer.Name, chatMessageFromServer.Message);
        }

        void IReceiver.CommandSorter(string message)
        {
            var command = JsonConvert.DeserializeObject<CommandMessage>(message);
            switch (command.CommandType)
            {
                case CommandType.Start:
                    
                    self.Start(command);
                    break;

                case CommandType.SetUsername:
                    self.SetUserName(command);
                    break;

                case CommandType.Here:
                    self.Here(command);
                    break;

                case CommandType.Ping:
                    self.Ping(command);
                    break;

                case CommandType.Pong:
                    self.Pong(command);
                    break;

                case CommandType.Sendfile:
                    self.SendFile(command);
                    break;
            }
        }

        void IReceiver.Start(CommandMessage command)
        {
            var file = filePath + "ClientProperties.txt";
            if (File.Exists(file))
            {
                var accessTokenForFile = command.Args["AccessToken"].ToString();
                File.WriteAllText(file, accessTokenForFile);
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start callback. Your access token is: {0}", command.Args["AccessToken"].ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        void IReceiver.SetUserName(CommandMessage command)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your name has been successfully changed. Now you are: {0}", command.Args["Name"]);
            Console.ForegroundColor = ConsoleColor.White;
        }

        void IReceiver.Here(CommandMessage command)
        {
            var userList = command.Args["Users"].ToString();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Online users: {0}", userList);
            Console.ForegroundColor = ConsoleColor.White;
        }

        void IReceiver.SendFile(CommandMessage command)
        {
            var extension = command.Args["Extension"];
            var guid = Guid.NewGuid().ToString();
            var bytes = Convert.FromBase64String(command.Args["File"].ToString());
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You recived file");
            Console.ForegroundColor = ConsoleColor.White;
            File.WriteAllBytes($"{filePath}{guid + "." + extension}", bytes);
        }

        void IReceiver.Ping(CommandMessage command)
        {
            command.CommandType = CommandType.Pong;

            sender.Launch(command);
        }

        void IReceiver.Pong(CommandMessage command)
        {
            autoPinger.AutoPingAnswerer = true;
            command.CommandType = CommandType.Pong;

            autoPinger.PongCallback();
        }

        void IReceiver.AbortThread()
        {
            Thread.Abort();
        }
    }
}
