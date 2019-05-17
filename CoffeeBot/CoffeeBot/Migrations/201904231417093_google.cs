namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class google : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Places", "GoogleID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("coffee.Places", "GoogleID");
        }
    }
}
