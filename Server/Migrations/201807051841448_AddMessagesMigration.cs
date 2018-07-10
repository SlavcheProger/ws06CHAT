namespace Chat.Socket.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessagesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessageFromCLientDBs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SenderID = c.Int(nullable: false),
                        RecipientID = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ChatMessageFromCLientDBs");
        }
    }
}
