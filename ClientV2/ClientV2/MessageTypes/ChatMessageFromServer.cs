namespace Chat.Socket.ClientV2.MessageTypes
{
    class ChatMessageFromServer : BaseMessage
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public ChatMessageFromServer()
        {

        }

    }
}
