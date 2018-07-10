using System;
using System.Net.Sockets;
using System.Threading;

namespace Client1
{
    internal class ChatLauncher : IDisconnectable
    {
        private Thread receiveThread;
        private ChatCore chatCore;
        private ServerConnection connection;
        internal ChatLauncher(){}

        internal void Launch()
        {
            try
            {
                connection = new ServerConnection();
            }catch(Exception exception)
            {
                var logger = new Logger();
                logger.SaveLog(exception);
                var displayService = new DisplayMessageService();
                displayService.Display(exception.Message, DisplayMessageType.Error);
            }
            if (connection == null)
            {
                Disconnect();
            }
            chatCore = new ChatCore(this, connection.stream);
            var displayMessage = new DisplayMessageService();
            displayMessage.Display("Connected.", DisplayMessageType.Success);
            var receiver = new Receiver(connection.stream, this, chatCore);

            receiveThread = new Thread(() => receiver.ReceiveMessage());
            receiveThread.Start();

            var commandHandler = new InputHandler(connection.stream, this, chatCore);
            commandHandler.ConsoleReading();

            receiveThread.Join();
        }
        
        public void Disconnect()
        {

            connection?.CloseConnection();

            receiveThread?.Abort();
            var display = new DisplayMessageService();
            display.Display("You were disconnected.", DisplayMessageType.Error);

            var chatStart = new ChatLauncher();
            chatStart.Launch();
        }
    }
}