using System;
namespace ClientV5.Services.Interfaces
{
    public interface ILogger
    {
        void Write(string message);

        void Write(Exception exception);
    }
}
