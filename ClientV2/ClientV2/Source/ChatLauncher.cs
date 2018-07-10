using Chat.Socket.ClientV2.MessageTypes;
using Chat.Socket.ClientV2.Services;
using Chat.Socket.ClientV2.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace Chat.Socket.ClientV2.Source
{

    class ChatLauncher
    {
        private ServerConnection Connector;

        private readonly ISender sender;
        private readonly IReceiver receiver;
        private readonly IConsoleDataHandler consoleDataHandler;
        private readonly IAutoPinger autoPinger;
        private readonly IDisconnector disconnector;

        internal ChatLauncher()
        {
            sender = DependencyResolver.Get<ISender>();
            disconnector = DependencyResolver.Get<IDisconnector>();
            
            consoleDataHandler = DependencyResolver.Get<IConsoleDataHandler>();
            receiver = DependencyResolver.Get<IReceiver>();
            disconnector.Configure(ServerConnection.Stream, ServerConnection.client, receiver.Thread, consoleDataHandler.Thread);
            autoPinger = DependencyResolver.Get<IAutoPinger>();
        }

        internal void Start()
        {
            try
            {
                Connector = new ServerConnection("192.168.1.135", 8888);
                Connector.SetUp();

                consoleDataHandler.Thread = new Thread(() => consoleDataHandler.DataHandlerForSending());
                consoleDataHandler.Thread.Start();

                var commandMessage = new CommandMessage();

                if (File.Exists("../../ClientFiles/ClientProperties.txt"))
                {
                    var accessToken = File.ReadAllText("../../ClientFiles/ClientProperties.txt");
                    commandMessage.Args["AccessToken"] = accessToken;
                }
                else
                {
                    commandMessage.Args["AccessToken"] = "";
                    File.WriteAllText("../../ClientFiles/ClientProperties.txt", commandMessage.Args["AccessToken"].ToString());
                }

                
                var startMessage = JsonConvert.SerializeObject(commandMessage);

                sender.Launch(commandMessage);

                Console.WriteLine("Connected.");

                receiver.Thread = new Thread(() => receiver.ReceiveMessage());
                receiver.Thread.Start();
                
                autoPinger.AutoPingStart();

                receiver.Thread.Join();
                consoleDataHandler.Thread.Join();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }


    }
}
