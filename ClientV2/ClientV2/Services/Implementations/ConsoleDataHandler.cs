using Chat.Socket.ClientV2.MessageTypes;
using Chat.Socket.ClientV2.Services.Interfaces;
using Chat.Socket.ClientV2.Source;
using System;
using System.Threading;

namespace Chat.Socket.ClientV2.Services.Implementations
{
    class ConsoleDataHandler : IConsoleDataHandler
    {
        public Thread Thread { get; set; }

        private IConsoleDataHandler self;

        private readonly ISender sender;
        private readonly IDisconnector disconnector;
        private readonly IAutoPinger autoPinger;

        public ConsoleDataHandler()
        {
            self = this as IConsoleDataHandler;
            sender = DependencyResolver.Get<ISender>();
            disconnector = DependencyResolver.Get<IDisconnector>();
            autoPinger = DependencyResolver.Get<IAutoPinger>();
        }


        void IConsoleDataHandler.CommonMessage(string message)
        {
            var chatMessageFromClient = new ChatMessageFromClient
            {
                MessageType = MessageType.ChatMessageFromClient
            };

            if (message.Length > 2 && message[0] == '@')
            {
                message = message.Remove(0, 1);
                var wordArray = message.Split(' ');
                chatMessageFromClient.Args["Recipient"] = wordArray[0];
                var recipient = wordArray[0].ToString().Length;
                message = string.Join(" ", wordArray);
                message = message.Remove(0, recipient + 1);
            }

            chatMessageFromClient.Message = message;

            sender.Launch(chatMessageFromClient);
        }

        void IConsoleDataHandler.DataHandlerForSending()
        {
            while (true)
            {
                var data = Console.ReadLine();

                data = data.TrimStart(' ');

                if (!String.IsNullOrEmpty(data) && data[0] == '/')
                {
                    var splittedString = data.Split(' ');
                    splittedString[0] = splittedString[0].ToLower();

                    self.Sorter(splittedString);
                }
                else
                {
                    self.CommonMessage(data);
                }
            }
        }

        void IConsoleDataHandler.Here()
        {
            var command = new CommandMessage
            {
                CommandType = CommandType.Here
            };

            sender.Launch(command);
        }

        void IConsoleDataHandler.Ping()
        {
            var command = new CommandMessage();
            if (autoPinger.AutoPingAnswerer)
            {
                autoPinger.AutoPongAnswerer = false;
                command.CommandType = CommandType.Ping;
                Random rand = new Random();
                command.Args["ID"] = rand.Next(1000);

                sender.Launch(command);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You are already send ping command");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        void IConsoleDataHandler.SendFile(string[] splittedString)
        {
            var command = new CommandMessage();
            if (splittedString.Length > 1)
            {
                command.Args["Recipient"] = splittedString[1];
            }

            FileHandler.SendFile(command);
        }

        void IConsoleDataHandler.SetUserName(string[] wordArray)
        {
            var command = new CommandMessage();
            if (wordArray.Length > 1)
            {
                command.Args["Name"] = wordArray[1];
                command.CommandType = CommandType.SetUsername;

                sender.Launch(command);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter your name.");
            }
        }

        void IConsoleDataHandler.Sorter(string[] splittedString)
        {
            switch (splittedString[0])
            {
                case Commands.SetUserName:
                    self.SetUserName(splittedString);
                    break;
                case Commands.Here:
                    self.Here();
                    break;
                case Commands.Ping:
                    self.Ping();
                    break;
                case Commands.Disconnect:
                    disconnector.Disconnect();
                    break;
                case Commands.SendFile:
                    self.SendFile(splittedString);
                    break;
                default:
                    self.CommonMessage(String.Join(" ",splittedString));
                    break;
            }
        }

    }
}
