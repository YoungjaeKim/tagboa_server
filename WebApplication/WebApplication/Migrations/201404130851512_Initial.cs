namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UrlLinks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        IsBroken = c.Boolean(nullable: false),
                        IsHidden = c.Boolean(nullable: false),
                        Note = c.String(),
                        Item_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Items", t => t.Item_ID)
                .Index(t => t.Item_ID);
            
            AddColumn("dbo.Tags", "IsCurricular", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tags", "Item_ID", c => c.Int());
            CreateIndex("dbo.Tags", "Item_ID");
            AddForeignKey("dbo.Tags", "Item_ID", "dbo.Items", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.UrlLinks", "Item_ID", "dbo.Items");
            DropIndex("dbo.Tags", new[] { "Item_ID" });
            DropIndex("dbo.UrlLinks", new[] { "Item_ID" });
            DropColumn("dbo.Tags", "Item_ID");
            DropColumn("dbo.Tags", "IsCurricular");
            DropTable("dbo.UrlLinks");
        }
    }
}
