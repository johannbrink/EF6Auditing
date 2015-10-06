namespace EF6Auditing.UnitTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "audit.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        EventDateTime = c.DateTime(nullable: false),
                        EventType = c.String(),
                        SchemaName = c.String(),
                        TableName = c.String(),
                        KeyNames = c.String(),
                        KeyValues = c.String(),
                        ColumnName = c.String(),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                    })
                .PrimaryKey(t => t.AuditLogId);
            
            CreateTable(
                "dbo.MyCustomers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MyCustomers");
            DropTable("audit.AuditLogs");
        }
    }
}
