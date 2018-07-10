namespace Chat.Socket.ClientV2.MessageTypes
{
    class ServerNotification : BaseMessage
    {
        public string Message { set; get; }
        public ServerNotification(){}
    }
}
