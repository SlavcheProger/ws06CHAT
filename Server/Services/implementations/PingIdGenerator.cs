using Chat.Socket.Server.Services.Interfaces;

namespace Chat.Socket.Server.Services.Implementations
{
    public class PingIdGenerator : IPingIdGenerator
    {
        private static int lastPingId;

        static PingIdGenerator()
        {
            lastPingId = 1;
        }

        public PingIdGenerator() { }

        int IPingIdGenerator.NextPingIdGenerator => lastPingId++;

        int IPingIdGenerator.CurrentPingIdGenerator => lastPingId - 1;
    }
}
