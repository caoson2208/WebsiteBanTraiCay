namespace WebsiteBanTraiCay05.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTB : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "Expiry");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "Expiry", c => c.DateTime(nullable: false));
        }
    }
}
