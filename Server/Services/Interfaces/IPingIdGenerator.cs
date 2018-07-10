namespace Chat.Socket.Server.Services.Interfaces
              
{
    public interface IPingIdGenerator
    {
        int NextPingIdGenerator { get; }
        int CurrentPingIdGenerator { get; }
    }
}
