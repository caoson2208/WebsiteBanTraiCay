namespace WebsiteBanTraiCay05.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Review", "Rate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Review", "Rate", c => c.Int(nullable: false));
        }
    }
}
