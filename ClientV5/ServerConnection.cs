using System;
using System.Net.Sockets;
using System.Threading;
using Chat.Socket.Server.Services;
using ClientV5.Services.Interfaces;

namespace ClientV5
{
    public class ServerConnection
    {
        private TcpClient Client;
        private NetworkStream Stream;
        private Thread recieveThread;
        private Thread commandHandleThread;
		private readonly System.Timers.Timer timer;

		private string Ip { get; set; }
        private int Port { get; set; }

        private readonly IReciever reciever;
        private readonly ISender sender;
        private readonly ICommandHandler commandHandler;
        private readonly IDisconnector disconnector;

        public ServerConnection(string ip, int port) 
        {
            Ip = ip;
            Port = port;
            Client = new TcpClient();

            reciever = DependencyResolver.Get<IReciever>();
            sender = DependencyResolver.Get<ISender>();
            commandHandler = DependencyResolver.Get<ICommandHandler>();
            disconnector = DependencyResolver.Get<IDisconnector>();

			timer = new System.Timers.Timer(60 * 1000);
			timer.Elapsed += delegate { sender.Ping(); };
			timer.AutoReset = true;
			timer.Start();
		}

        internal void Launch()
        {
            Client.Connect(Ip, Port);
            Stream = Client.GetStream();
            Console.WriteLine("Connected");

            reciever.SetStream(Stream);
            sender.SetStream(Stream);

            recieveThread = new Thread(() => reciever.GettigMessageProcess());
            recieveThread.Start();

            commandHandleThread = new Thread(() => commandHandler.GettingMessagesProcess());
            commandHandleThread.Start();

            disconnector.Configure(Stream, recieveThread, commandHandleThread, Client);

            commandHandleThread.Join();
            recieveThread.Join();
        }
    }
}
