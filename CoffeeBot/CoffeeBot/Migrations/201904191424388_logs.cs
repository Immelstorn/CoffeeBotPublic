namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        Message = c.String(),
                        Addtitional = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("coffee.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("coffee.Logs", "User_Id", "coffee.Users");
            DropIndex("coffee.Logs", new[] { "User_Id" });
            DropTable("coffee.Logs");
        }
    }
}
