namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flagEnumForPerks : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Places", "Perks", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("coffee.Places", "Perks");
        }
    }
}
