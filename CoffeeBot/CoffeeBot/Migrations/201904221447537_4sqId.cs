namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4sqId : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Places", "FoursquareID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("coffee.Places", "FoursquareID");
        }
    }
}
