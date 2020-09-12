namespace MvcIndentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initalMine : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Note = c.String(storeType: "ntext"),
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(maxLength: 200),
                        Title = c.String(nullable: false),
                        Category = c.Int(nullable: false),
                        Viewcount = c.Int(nullable: false),
                        Published = c.DateTime(nullable: false),
                        Released = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Birth = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Body = c.String(),
                        Updated = c.DateTime(nullable: false),
                        ArticleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(),
                        Birth = c.DateTime(nullable: false),
                        Married = c.Boolean(nullable: false),
                        Memo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address_Prefecture = c.String(),
                        Address_City = c.String(),
                        Address_Street = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AuthorArticles",
                c => new
                    {
                        Author_Id = c.Int(nullable: false),
                        Article_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Author_Id, t.Article_Id })
                .ForeignKey("dbo.Authors", t => t.Author_Id, cascadeDelete: true)
                .ForeignKey("dbo.Articles", t => t.Article_Id, cascadeDelete: true)
                .Index(t => t.Author_Id)
                .Index(t => t.Article_Id);
            
            //DropTable("dbo.AspNetRoles");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUserLogins");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId });
            
            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            MyPage = c.String(maxLength: 128),
            //            Twitter = c.String(maxLength: 32),
            //            Email = c.String(maxLength: 256),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId });
            
            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 256),
            //            RoleFlags = c.Int(),
            //            Discriminator = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Comments", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.AuthorArticles", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.AuthorArticles", "Author_Id", "dbo.Authors");
            DropIndex("dbo.AuthorArticles", new[] { "Article_Id" });
            DropIndex("dbo.AuthorArticles", new[] { "Author_Id" });
            DropIndex("dbo.Comments", new[] { "ArticleId" });
            DropTable("dbo.AuthorArticles");
            DropTable("dbo.People");
            DropTable("dbo.Members");
            DropTable("dbo.Comments");
            DropTable("dbo.Authors");
            DropTable("dbo.Articles");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            CreateIndex("dbo.AspNetUserClaims", "UserId");
            CreateIndex("dbo.AspNetUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetRoles", "Name", unique: true, name: "RoleNameIndex");
            //AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
        }
    }
}
