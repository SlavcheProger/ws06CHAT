using System;
using System.Net.Sockets;
using System.Text;

namespace Client1
{
    public class Receiver
    {
        private NetworkStream stream;

        private IDisconnectable disconnector;

        private ChatCore chatCore { get; set; }

        internal Receiver(NetworkStream networkStream, IDisconnectable disconnector, ChatCore chatCore)
        {
            this.disconnector = disconnector;
            stream = networkStream;
            this.chatCore = chatCore;
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    var data = new byte[2097152];
                    var stringBuilder = new StringBuilder();
                    var bytes = 0;
                    while (stream.DataAvailable)
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        stringBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }

                    var message = stringBuilder.ToString();
                    if (!string.IsNullOrEmpty(message))
                    {
                        chatCore.ReceiveHandler(message);
                    }
                }
                catch (Exception exception)
                {
                    var log = new Logger();
                    log.SaveLog(exception);
                    //var display = new DisplayMessageService();
                    //display.Display(DisplayMessageType.IncorrectCommand, "Can't receive data.");
                    //display.Display(DisplayMessageType.IncorrectCommand, exception.Message);
                    disconnector.Disconnect();
                }
            }
        }

    }
}