namespace Chat.Socket.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFilesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileDBs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SenderId = c.Int(nullable: false),
                        RecipientId = c.Int(nullable: false),
                        Filename = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileDBs");
        }
    }
}
