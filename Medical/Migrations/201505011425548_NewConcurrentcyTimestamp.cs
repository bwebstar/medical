namespace Medical.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewConcurrentcyTimestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "Timestamp");
        }
    }
}
