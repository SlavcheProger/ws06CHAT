using System;
using System.IO;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
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
            File.WriteAllText("client.log", exception.ToString());
        }
    }
}
