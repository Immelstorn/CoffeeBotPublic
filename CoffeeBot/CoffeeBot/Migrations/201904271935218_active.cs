namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class active : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Places", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("coffee.Places", "Active");
        }
    }
}
