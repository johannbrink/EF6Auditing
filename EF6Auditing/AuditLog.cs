using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EF6Auditing
{
    [Table(name: "AuditLogs", Schema = "audit")]
    public class AuditLog
    {
        [Key]
        public long AuditLogId { get; set; }

        public string UserName { get; set; }
        
        public DateTime EventDateTime { get; set; }

        public string EventType { get; set; }

        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public string KeyNames { get; set; }

        public string KeyValues { get; set; }

        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }
    }
}
