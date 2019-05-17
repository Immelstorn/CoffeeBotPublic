namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class country : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Cities", "Country", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("coffee.Cities", "Country");
        }
    }
}
