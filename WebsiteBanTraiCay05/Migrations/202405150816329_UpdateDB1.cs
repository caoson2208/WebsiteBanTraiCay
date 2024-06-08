namespace WebsiteBanTraiCay05.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Review", "ProductId");
            AddForeignKey("dbo.Review", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Review", "ProductId", "dbo.Product");
            DropIndex("dbo.Review", new[] { "ProductId" });
        }
    }
}
