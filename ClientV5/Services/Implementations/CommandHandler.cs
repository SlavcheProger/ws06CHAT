using System;
using System.Text.RegularExpressions;
using Chat.Socket.Server.Services;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class CommandHandler: ICommandHandler
    {
        private ISender sender; 
        private readonly ICommandHandler self;
        private readonly IDisconnector disconnector;
        private readonly IFileSelector fileSelector;

        public CommandHandler()
        {
            sender = DependencyResolver.Get<ISender>();
            disconnector = DependencyResolver.Get<IDisconnector>();
            fileSelector = DependencyResolver.Get<IFileSelector>();
            self = this as ICommandHandler;
        }

        void ICommandHandler.GettingMessagesProcess()
        {
            sender.AuthorizeRequest();

            string data;

            while (true)
            {
                data = Console.ReadLine();
                self.Parse(data);
            }
        }

        void ICommandHandler.Parse(string data)
        {
            var dataCopy = Regex.Replace(data.Trim(), @"\s+", " ");
            var dataSplitted = dataCopy.Split(' ');

            switch (dataSplitted[0].ToLower())
            {
                case "/start":
                    sender.AuthorizeRequest();
                    break;

                case "/setusername":
                    if (dataSplitted.Length > 1)
                    {
                        sender.SetUsername(dataSplitted[1]);
                    }
                    else
                    {
                        sender.SetUsername(string.Empty);
                    }
                    break;

                case "/here":
                    sender.Here();
                    break;

                case "/sendfile":
                    var transportContainer = fileSelector.GetDataFromFile();

                    if (transportContainer.Args.ContainsKey("File"))
                    {
                        if(dataSplitted.Length > 1)
                        {
                            sender.SendFile(transportContainer, dataSplitted[1]);
                        }
                        else
                        {
                            sender.SendFile(transportContainer);
                        }
                    }
                    break;

                case "/disconnect":
                    disconnector.Disconnect();
                    break;
                    

                default:
                    if (dataSplitted.Length > 1 && dataSplitted[0][0] == '@')
                    {
                        sender.ChatMessage(data.Remove(0,dataSplitted[0].Length + 1), dataSplitted[0].Remove(0, 1));
                    }
                    else
                    {
                        sender.ChatMessage(data);
                    }
                    break;
            }
        }
    }
}
