namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setWorkingTimeToNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("coffee.Places", "OpenTime", c => c.Time(precision: 7));
            AlterColumn("coffee.Places", "OpenTimeWeekend", c => c.Time(precision: 7));
            AlterColumn("coffee.Places", "CloseTime", c => c.Time(precision: 7));
            AlterColumn("coffee.Places", "CloseTimeWeekend", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("coffee.Places", "CloseTimeWeekend", c => c.Time(nullable: false, precision: 7));
            AlterColumn("coffee.Places", "CloseTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("coffee.Places", "OpenTimeWeekend", c => c.Time(nullable: false, precision: 7));
            AlterColumn("coffee.Places", "OpenTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
