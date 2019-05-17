namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("coffee.Places", "Longitude", c => c.Double(nullable: false));
            AlterColumn("coffee.Places", "Latitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("coffee.Places", "Latitude", c => c.Single(nullable: false));
            AlterColumn("coffee.Places", "Longitude", c => c.Single(nullable: false));
        }
    }
}
