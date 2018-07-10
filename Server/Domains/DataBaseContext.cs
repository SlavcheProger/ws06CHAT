using Chat.Socket.Server.Domains.Messages;
using System.Data.Entity;

namespace Chat.Socket.Server.Domains.Accounts
{
    class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("DBConnection") { }

        public DbSet<UserDB> Users { get; set; }
        public DbSet<ChatMessageFromCLientDB> ChatMessages { get; set; }
        public DbSet<FileDB> Files { get; set; }
    }
}