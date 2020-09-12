namespace MvcIndentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentRequire : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "Body", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "Body", c => c.String());
        }
    }
}
