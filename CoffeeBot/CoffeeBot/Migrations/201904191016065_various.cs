namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class various : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Ratings", "NeedReview", c => c.Boolean(nullable: false));
            AddColumn("coffee.Users", "UserModeParams", c => c.String());
            AlterColumn("coffee.Ratings", "Stars", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("coffee.Ratings", "Stars", c => c.Byte(nullable: false));
            DropColumn("coffee.Users", "UserModeParams");
            DropColumn("coffee.Ratings", "NeedReview");
        }
    }
}
