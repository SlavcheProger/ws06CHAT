using System;
using System.Net.Sockets;

namespace Client1
{
    internal class InputHandler
    {
        private NetworkStream stream;
        private IDisconnectable disconnector;
        private ChatLauncher connection { get; set; }
        private ChatCore chatCore { get; set; }

        internal InputHandler(NetworkStream networkStream, IDisconnectable disconnector, ChatCore chatCore)
        {
            stream = networkStream;
            this.disconnector = disconnector;
            this.chatCore = chatCore;
        }

        internal void ConsoleReading()
        {
            
            HandleInput(Commands.Start);
            while (true)
            {
                var message = Console.ReadLine();
                HandleInput(message);
            }
        }

        private void HandleInput(string message)
        {            
            message.Trim();
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (message.Length != 1)
            {
                if (message[1] != ' ')
                {
                    if (message[0] == '@')
                    {
                        chatCore.SendToUserHandler(message);
                        return;
                    }
                    else if (message[0] == '/')
                    {
                        chatCore.CommandHandler(message);
                        return;
                    }
                }
            }
            chatCore.SendMessage(message);
        }
    }
}
