namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.UserSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Language = c.Int(nullable: false, defaultValue: 1),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("coffee.Users", "Settings_Id", c => c.Int());
            CreateIndex("coffee.Users", "Settings_Id");
            AddForeignKey("coffee.Users", "Settings_Id", "coffee.UserSettings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("coffee.Users", "Settings_Id", "coffee.UserSettings");
            DropIndex("coffee.Users", new[] { "Settings_Id" });
            DropColumn("coffee.Users", "Settings_Id");
            DropTable("coffee.UserSettings");
        }
    }
}
