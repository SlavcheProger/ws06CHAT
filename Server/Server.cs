using System.Net.Sockets;
using System.Threading;
using Chat.Socket.Server.Services;
using Chat.Socket.Server.Services.Interfaces;
using System.Net;
using System;
using Chat.Socket.Server.Domains.Accounts;
using System.IO;

namespace Chat.Socket.Server
{
    public class Server
    {
        // Services
        private readonly IUsersService usersService;
        private readonly IConnectionsService connectionsService;
        private readonly ILogger logger;

        // Data
        private readonly Thread processThread;
        private readonly TcpListener listener;

        public Server()
        {
            usersService = DependencyResolver.Get<IUsersService>();
            connectionsService = DependencyResolver.Get<IConnectionsService>();
            logger = DependencyResolver.Get<ILogger>();

            processThread = new Thread(() => Listen());
            listener = new TcpListener(IPAddress.Any, 8888);
        }

        private void Configure()
        {
            using (var context = new DataBaseContext())
            {
                try
                {
                    var users = context.Users;

                    foreach (var user in users)
                    {
                        user.Status = Status.Offline;
                    }
                }
                catch (Exception exception)
                {
                    logger.Write(exception);
                }
            }

            if (!Directory.Exists("./../../usersfiles"))
            {
                Directory.CreateDirectory("./../../usersfiles");
            }
        }

        internal void Launch()
        {
            Configure();

            processThread.Start();
            listener.Start();

            processThread.Join();
        }

        private void Listen()
        {
            logger.Write("Server started.");
            TcpClient tcpClient;

            while (true)
            {
                try
                {
                    tcpClient = listener.AcceptTcpClient();
                    logger.Write("New connection.");

                    ProcessNewConnection(tcpClient);
                }
                catch (Exception exception)
                {
                    logger.Write("Problem with accept tcp client.");
                    logger.Write(exception);
                }
            }
        }

        private void ProcessNewConnection(TcpClient client)
        {
            var connection = new ClientConnection(client);
            connectionsService.Add(connection);

            connection.Launch();
        }
    }
}