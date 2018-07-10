using System;
using System.IO;
using Chat.Socket.Server.Services.Interfaces;

namespace Chat.Socket.Server.Services.Implementations
{
    public class Logger: ILogger
    {
        public Logger() {}

        void ILogger.Write(string message)
        {
            Console.WriteLine(message);
        }

        void ILogger.Write(Exception exception)
        {
            File.WriteAllText("server.log", exception.Message);
        }
    }
}
