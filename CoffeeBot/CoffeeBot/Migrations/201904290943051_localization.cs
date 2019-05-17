namespace CoffeeBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class localization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "coffee.Localizations",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "coffee.LocalizationTexts",
                c => new
                    {
                        Localization_Name = c.String(nullable: false, maxLength: 128),
                        Text_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Localization_Name, t.Text_Id })
                .ForeignKey("coffee.Localizations", t => t.Localization_Name, cascadeDelete: true)
                .ForeignKey("coffee.Texts", t => t.Text_Id, cascadeDelete: true)
                .Index(t => t.Localization_Name)
                .Index(t => t.Text_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("coffee.LocalizationTexts", "Text_Id", "coffee.Texts");
            DropForeignKey("coffee.LocalizationTexts", "Localization_Name", "coffee.Localizations");
            DropIndex("coffee.LocalizationTexts", new[] { "Text_Id" });
            DropIndex("coffee.LocalizationTexts", new[] { "Localization_Name" });
            DropTable("coffee.LocalizationTexts");
            DropTable("coffee.Localizations");
        }
    }
}
