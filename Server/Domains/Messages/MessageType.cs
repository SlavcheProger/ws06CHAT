namespace Chat.Socket.Server.Domains.Messages
{
    public enum MessageType
    {
        CommandMessage = 1,
        ServerNotification = 2,
        ChatMessageFromClient = 3,
        ChatMessageFromServer = 4
    }
}
