using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace EF6Auditing
{
    /// <summary>
    /// The AuditLogBuilder class is used internally by AuditingDbContext.
    /// </summary>
    internal static class AuditLogBuilder
    {
        private const string KeySeperator = ";";

        internal static List<AuditLog> GetAuditLogsForExistingEntities(string userName,
            IEnumerable<DbEntityEntry> modifiedEntityEntries,
            IEnumerable<DbEntityEntry> deletedEntityEntries, 
            ObjectContext objectContext)
        {
            var auditLogs = new List<AuditLog>();
            foreach (
                var auditRecordsForEntityEntry in
                    modifiedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Modified, objectContext)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            foreach (
                var auditRecordsForEntityEntry in
                    deletedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Deleted, objectContext)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            return auditLogs;
        }

        internal static List<AuditLog> GetAuditLogsForAddedEntities(string userName,
            IEnumerable<DbEntityEntry> addedEntityEntries,
            ObjectContext objectContext)
        {
            var auditLogs = new List<AuditLog>();
            foreach (
                var auditRecordsForEntityEntry in
                    addedEntityEntries.Select(
                        changedEntity => GetAuditLogs(changedEntity, userName, EntityState.Added, objectContext)))
                auditLogs.AddRange(auditRecordsForEntityEntry);
            return auditLogs;
        }

        private static IEnumerable<AuditLog> GetAuditLogs(DbEntityEntry entityEntry, string userName, EntityState entityState, ObjectContext objectContext)
        {
            var returnValue = new List<AuditLog>();
            var keyRepresentation = BuildKeyRepresentation(entityEntry, KeySeperator, objectContext);

            var auditedPropertyNames =
                entityEntry.Entity.GetType()
                    .GetProperties().Where(p => !p.GetCustomAttributes(typeof(DoNotAudit), true).Any())
                    .Select(info => info.Name);
            foreach (var propertyName in auditedPropertyNames)
            {
                var currentValue = Convert.ToString(entityEntry.CurrentValues.GetValue<object>(propertyName));
                var originalValue = Convert.ToString(entityEntry.OriginalValues.GetValue<object>(propertyName));
                if (entityState == EntityState.Modified)
                    if (originalValue == currentValue) //Values are the same, don't log
                        continue;
                returnValue.Add(new AuditLog
                {
                    KeyNames = keyRepresentation.Key,
                    KeyValues = keyRepresentation.Value,
                    OriginalValue = entityState != EntityState.Added
                        ? originalValue
                        : null,
                    NewValue = entityState == EntityState.Modified || entityState == EntityState.Added
                        ? currentValue
                        : null,
                    ColumnName = propertyName,
                    EventDateTime = DateTime.Now,
                    EventType = entityState.ToString(),
                    UserName = userName,
                    TableName = entityEntry.Entity.GetType().Name
                });
            }
            return returnValue;
        }


        private static KeyValuePair<string, string> BuildKeyRepresentation(DbEntityEntry entityEntry, string seperator, ObjectContext objectContext)
        {
            var keyNames = GetKeyPropertyNames(entityEntry.Entity.GetType(), objectContext.MetadataWorkspace);
            var keyNameString = new StringBuilder();
            var keyValueString = new StringBuilder();
            foreach (var keyName in keyNames)
            {
                keyNameString.Append(keyName);
                keyNameString.Append(seperator);
                keyValueString.Append(Convert.ToString(entityEntry.CurrentValues.GetValue<object>(keyName)));
                keyValueString.Append(seperator);
            }
            keyNameString.Remove(keyNameString.Length - 1, 1);
            keyValueString.Remove(keyValueString.Length - 1, 1);
            var key = keyNameString.ToString();
            var value = keyValueString.ToString();
            return new KeyValuePair<string, string>(key, value);
        }

        private static IEnumerable<string> GetKeyPropertyNames(Type type, MetadataWorkspace workspace)
        {
            EdmType edmType;

            if (workspace.TryGetType(type.Name, type.Namespace, DataSpace.OSpace, out edmType))
            {
                return edmType.MetadataProperties.Where(mp => mp.Name == "KeyMembers")
                    .SelectMany(mp => mp.Value as ReadOnlyMetadataCollection<EdmMember>)
                    .OfType<EdmProperty>().Select(edmProperty => edmProperty.Name);
            }

            return null;
        }
    }
}
