namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usernames : DbMigration
    {
        public override void Up()
        {
            AddColumn("coffee.Users", "FirstName", c => c.String());
            AddColumn("coffee.Users", "LastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("coffee.Users", "LastName");
            DropColumn("coffee.Users", "FirstName");
        }
    }
}
