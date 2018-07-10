namespace Chat.Socket.Server.Services.Interfaces
{
    public interface ICommandHandler
    {
        void Start(ClientConnection connection, string accessToken);

        void Here(ClientConnection connection);

        void SetUsername(ClientConnection connection, string name);
    }
}
