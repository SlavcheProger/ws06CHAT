using System.ComponentModel.DataAnnotations;

namespace Chat.Socket.Server.Domains.Messages
{
    class FileDB
    {
        [Key]
        public int ID { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Filename { get; set; }

        public FileDB() : this(0, 0, string.Empty) { }
        public FileDB(int senderId, int recipientId, string filename)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Filename = filename;
        }
    }
}
