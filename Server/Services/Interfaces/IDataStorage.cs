namespace Chat.Socket.Server.Services.Interfaces
{
    public interface IDataStorage
    {
        void SaveUser(string username, string accessToken);

        void SaveMessage(string chatMessage, int recipientId, int senderId);

        void SaveFile(int senderId, int recipientId, byte[] contentOfFile, string extension);
    }
}
