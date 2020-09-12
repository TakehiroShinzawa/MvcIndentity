namespace MvcIndentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Authors", "Birth", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Authors", "Birth", c => c.DateTime(nullable: false));
        }
    }
}
