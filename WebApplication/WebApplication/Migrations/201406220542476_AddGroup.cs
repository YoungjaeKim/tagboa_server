namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.RoleInGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Role = c.String(),
                        User_Id = c.String(maxLength: 128),
                        Group_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .ForeignKey("dbo.Groups", t => t.Group_ID)
                .Index(t => t.User_Id)
                .Index(t => t.Group_ID);
            
            AddColumn("dbo.Curriculars", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Items", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Curriculars", "ApplicationUser_Id");
            CreateIndex("dbo.Items", "ApplicationUser_Id");
            AddForeignKey("dbo.Items", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Curriculars", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.RoleInGroups", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.RoleInGroups", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Curriculars", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Items", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.RoleInGroups", new[] { "Group_ID" });
            DropIndex("dbo.RoleInGroups", new[] { "User_Id" });
            DropIndex("dbo.Groups", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Items", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Curriculars", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Items", "ApplicationUser_Id");
            DropColumn("dbo.Curriculars", "ApplicationUser_Id");
            DropTable("dbo.RoleInGroups");
            DropTable("dbo.Groups");
        }
    }
}
