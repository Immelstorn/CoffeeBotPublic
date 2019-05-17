namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realmFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RealmID = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "coffee.Texts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Language = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "coffee.CityTexts",
                c => new
                    {
                        City_Id = c.Int(nullable: false),
                        Text_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.City_Id, t.Text_Id })
                .ForeignKey("coffee.Cities", t => t.City_Id, cascadeDelete: true)
                .ForeignKey("coffee.Texts", t => t.Text_Id, cascadeDelete: true)
                .Index(t => t.City_Id)
                .Index(t => t.Text_Id);
            
            CreateTable(
                "coffee.PlacesTextsAddress",
                c => new
                    {
                        Place_Id = c.Int(nullable: false),
                        Text_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Place_Id, t.Text_Id })
                .ForeignKey("coffee.Places", t => t.Place_Id, cascadeDelete: true)
                .ForeignKey("coffee.Texts", t => t.Text_Id, cascadeDelete: true)
                .Index(t => t.Place_Id)
                .Index(t => t.Text_Id);
            
            CreateTable(
                "coffee.PlacesTextsDescription",
                c => new
                    {
                        Place_Id = c.Int(nullable: false),
                        Text_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Place_Id, t.Text_Id })
                .ForeignKey("coffee.Places", t => t.Place_Id, cascadeDelete: true)
                .ForeignKey("coffee.Texts", t => t.Text_Id, cascadeDelete: true)
                .Index(t => t.Place_Id)
                .Index(t => t.Text_Id);
            
            AddColumn("coffee.Places", "OpenTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("coffee.Places", "OpenTimeWeekend", c => c.Time(nullable: false, precision: 7));
            AddColumn("coffee.Places", "CloseTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("coffee.Places", "CloseTimeWeekend", c => c.Time(nullable: false, precision: 7));
            AddColumn("coffee.Places", "RealmID", c => c.Int(nullable: false));
            AddColumn("coffee.Places", "City_Id", c => c.Int());
            CreateIndex("coffee.Places", "City_Id");
            AddForeignKey("coffee.Places", "City_Id", "coffee.Cities", "Id");
            DropColumn("coffee.Places", "Description");
            DropColumn("coffee.Places", "Address");
        }
        
        public override void Down()
        {
            AddColumn("coffee.Places", "Address", c => c.String());
            AddColumn("coffee.Places", "Description", c => c.String());
            DropForeignKey("coffee.PlacesTextsDescription", "Text_Id", "coffee.Texts");
            DropForeignKey("coffee.PlacesTextsDescription", "Place_Id", "coffee.Places");
            DropForeignKey("coffee.Places", "City_Id", "coffee.Cities");
            DropForeignKey("coffee.PlacesTextsAddress", "Text_Id", "coffee.Texts");
            DropForeignKey("coffee.PlacesTextsAddress", "Place_Id", "coffee.Places");
            DropForeignKey("coffee.CityTexts", "Text_Id", "coffee.Texts");
            DropForeignKey("coffee.CityTexts", "City_Id", "coffee.Cities");
            DropIndex("coffee.PlacesTextsDescription", new[] { "Text_Id" });
            DropIndex("coffee.PlacesTextsDescription", new[] { "Place_Id" });
            DropIndex("coffee.PlacesTextsAddress", new[] { "Text_Id" });
            DropIndex("coffee.PlacesTextsAddress", new[] { "Place_Id" });
            DropIndex("coffee.CityTexts", new[] { "Text_Id" });
            DropIndex("coffee.CityTexts", new[] { "City_Id" });
            DropIndex("coffee.Places", new[] { "City_Id" });
            DropColumn("coffee.Places", "City_Id");
            DropColumn("coffee.Places", "RealmID");
            DropColumn("coffee.Places", "CloseTimeWeekend");
            DropColumn("coffee.Places", "CloseTime");
            DropColumn("coffee.Places", "OpenTimeWeekend");
            DropColumn("coffee.Places", "OpenTime");
            DropTable("coffee.PlacesTextsDescription");
            DropTable("coffee.PlacesTextsAddress");
            DropTable("coffee.CityTexts");
            DropTable("coffee.Texts");
            DropTable("coffee.Cities");
        }
    }
}
