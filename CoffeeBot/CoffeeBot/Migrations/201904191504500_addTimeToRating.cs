namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimeToRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Ratings", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("coffee.Ratings", "Timestamp");
        }
    }
}
