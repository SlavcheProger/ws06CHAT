using System;

namespace Chat.Socket.Server.Services.Interfaces
{
    public interface ILogger
    {
        void Write(string message);
        void Write(Exception exception);
    }
}
