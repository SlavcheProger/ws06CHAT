using System;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
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

		bool IPingIdGenerator.PingAnswered { get; set; }
	}
}
