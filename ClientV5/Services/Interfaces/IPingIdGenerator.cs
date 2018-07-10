using System;
namespace ClientV5.Services.Interfaces
{
    public interface IPingIdGenerator
    {
        int NextPingIdGenerator { get; }
        int CurrentPingIdGenerator { get; }
		bool PingAnswered { get; set; }
    }
}
