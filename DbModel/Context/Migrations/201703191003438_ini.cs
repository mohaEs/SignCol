namespace DbModel.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ini : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Languages",
            //    c => new
            //        {
            //            lang_id = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            word_id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.lang_id);
            
            //CreateTable(
            //    "dbo.Words",
            //    c => new
            //        {
            //            word_id = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //            WordType = c.Int(nullable: false),
            //            lang_id = c.Int(),
            //            User_id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.word_id)
            //    .ForeignKey("dbo.Languages", t => t.lang_id)
            //    .ForeignKey("dbo.User", t => t.User_id)
            //    .Index(t => t.lang_id)
            //    .Index(t => t.User_id);
            
            //CreateTable(
            //    "dbo.User",
            //    c => new
            //        {
            //            User_id = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //            Age = c.String(),
            //            Phone = c.String(),
            //            word_id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.User_id);
            
            //CreateTable(
            //    "dbo.Option",
            //    c => new
            //        {
            //            option_Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //            Value = c.String(),
            //            Date = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.option_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Words", "User_id", "dbo.User");
            DropForeignKey("dbo.Words", "lang_id", "dbo.Languages");
            DropIndex("dbo.Words", new[] { "User_id" });
            DropIndex("dbo.Words", new[] { "lang_id" });
            DropTable("dbo.Option");
            DropTable("dbo.User");
            DropTable("dbo.Words");
            DropTable("dbo.Languages");
        }
    }
}
