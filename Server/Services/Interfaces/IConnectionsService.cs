namespace Chat.Socket.Server.Services.Interfaces
{
    public interface IConnectionsService
    {
        ClientConnection[] Connections { get; }

        void Add(ClientConnection connection);
        void Remove(ClientConnection connection);
    }
}
