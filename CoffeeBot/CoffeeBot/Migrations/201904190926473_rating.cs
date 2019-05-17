namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rating : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Stars = c.Byte(nullable: false),
                        Comment = c.String(),
                        Place_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("coffee.Places", t => t.Place_Id)
                .ForeignKey("coffee.Users", t => t.User_Id)
                .Index(t => t.Place_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("coffee.Ratings", "User_Id", "coffee.Users");
            DropForeignKey("coffee.Ratings", "Place_Id", "coffee.Places");
            DropIndex("coffee.Ratings", new[] { "User_Id" });
            DropIndex("coffee.Ratings", new[] { "Place_Id" });
            DropTable("coffee.Ratings");
        }
    }
}
