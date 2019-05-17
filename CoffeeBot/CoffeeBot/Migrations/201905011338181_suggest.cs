namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class suggest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.Suggestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("coffee.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("coffee.Suggestions", "User_Id", "coffee.Users");
            DropIndex("coffee.Suggestions", new[] { "User_Id" });
            DropTable("coffee.Suggestions");
        }
    }
}
