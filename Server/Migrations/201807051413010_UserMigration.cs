namespace Chat.Socket.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "UserDBs");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.UserDBs", newName: "Users");
        }
    }
}
