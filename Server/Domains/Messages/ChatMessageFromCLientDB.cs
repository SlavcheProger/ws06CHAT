using System.ComponentModel.DataAnnotations;

namespace Chat.Socket.Server.Domains.Messages
{
    class ChatMessageFromCLientDB
    {
        [Key]
        public int ID { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public string Text { get; set; }

        public ChatMessageFromCLientDB() : this(0, 0, string.Empty) { }
        public ChatMessageFromCLientDB(int senderID, int recipientID, string text)
        {
            SenderID = senderID;
            RecipientID = recipientID;
            Text = text;
        }
    }
}
