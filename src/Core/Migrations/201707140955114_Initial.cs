namespace Core.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Down()
        {
            DropTable("dbo.Settings");
        }

        public override void Up()
        {
            CreateTable(
                    "dbo.Settings",
                    c => new
                    {
                        Id = c.Int(false, true),
                        Name = c.String(false, 4000),
                        Value = c.String(maxLength: 4000)
                    })
                .PrimaryKey(t => t.Id);
        }
    }
}