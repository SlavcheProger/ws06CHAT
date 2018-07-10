using System;
using System.IO;
using Chat.Socket.Server.Services;
using ClientV5.Domains.Messages;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class ChatCore: IChatCore
    {
        private readonly IAccessTokenStorage accessTokenStorage;
        private readonly IDisplayMessageService displayMessageService;
        private readonly ISender sender;
        private readonly IPingIdGenerator pingIdGenerator;

        public ChatCore() 
        {
            accessTokenStorage = DependencyResolver.Get<IAccessTokenStorage>();
            displayMessageService = DependencyResolver.Get<IDisplayMessageService>();
            sender = DependencyResolver.Get<ISender>();
            pingIdGenerator = DependencyResolver.Get<IPingIdGenerator>();
			pingIdGenerator.PingAnswered = true;
        }

        void IChatCore.ChatMessageHandle(ChatMessageFromServerContainer chatMessage)
        {
            displayMessageService.Write($"{chatMessage.Name}: {chatMessage.Text}");
        }

        void IChatCore.CommandHandle(CommandMessageContainer commandMessage)
        {
            switch (commandMessage.Command)
            {
                case CommandType.Start:
                    if (commandMessage.Args.ContainsKey("AccessToken"))
                    {
                        accessTokenStorage.Set(commandMessage.Args["AccessToken"].ToString());
                    }
                    break;

                case CommandType.SetUsername:
                    if (commandMessage.Args.ContainsKey("Name") && !string.IsNullOrEmpty(commandMessage.Args["Name"].ToString()))
                    {
                        displayMessageService.Write($"Username successfully changed. Your current username: {commandMessage.Args["Name"].ToString()}");
                    }
                    else
                    {
                        displayMessageService.Write("Enter the name after /setusername command.");
                    }
                    break;

                case CommandType.Here:
                    if (commandMessage.Args.ContainsKey("Users"))
                    {
                        displayMessageService.Write(commandMessage.Args["Users"].ToString());
                    }
                    break;

                case CommandType.SendFile:
                    if (commandMessage.Args.ContainsKey("File") && commandMessage.Args.ContainsKey("Extension"))
                    {
                        var byteString = Convert.FromBase64String(commandMessage.Args["File"].ToString());
                        var filename = $"{Guid.NewGuid().ToString()}.{commandMessage.Args["Extension"].ToString()}";

                        File.WriteAllBytes(filename, byteString);
                    }

                    break;
                    
                case CommandType.Ping:
                    sender.Pong(commandMessage.Args);
                    break;

                case CommandType.Pong:
                    if (commandMessage.Args.ContainsKey("ID") && 
                        Int32.Parse(commandMessage.Args["ID"].ToString()) == pingIdGenerator.CurrentPingIdGenerator)
                    {
						pingIdGenerator.PingAnswered = true;
                    }
                    break;

                default:
                    break;
            }
        }

        void IChatCore.ServerNotificationHandle(ServerNotificationContainer serverNotification)
        {
            displayMessageService.Write($"Server Notification: {serverNotification.Text}");
        }
    }
}
