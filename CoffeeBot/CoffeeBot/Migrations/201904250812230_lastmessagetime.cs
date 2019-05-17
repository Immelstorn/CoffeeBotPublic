namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastmessagetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Users", "LastMessageTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("coffee.Users", "LastMessageTime");
        }
    }
}
