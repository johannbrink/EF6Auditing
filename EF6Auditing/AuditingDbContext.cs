using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EF6Auditing
{
    public abstract class AuditingDbContext : DbContext
    {


        public DbSet<AuditLog> AuditLogs { get; set; }

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the AuditingDbContext class (Extends Microsoft.Data.Entity.DbContext). 
        ///     This class writes Audit Logs to the current database using Entity Framework to the default table: [audit].[AuditLogs]
        ///     The Microsoft.Data.Entity.DbContext.OnConfiguring(Microsoft.Data.Entity.DbContextOptionsBuilder)
        ///     method will be called to configure the database (and other options) to be used
        ///     for this context.
        /// </summary>
        protected AuditingDbContext(string nameOrConnectionString) :base(nameOrConnectionString)
        {
           
        }

        #endregion

        #region Obsolete Base Members

        [Obsolete("A UserName is required. Use SaveChanges(string userName) instead.")]
        public new int SaveChanges()
        {
            throw new InvalidOperationException("A UserName is required. Use SaveChanges(string userName) instead.");
        }

        [Obsolete("A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.")]
        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException(
                "A UserName is required. Use SaveChangesAsync(userName, cancellationToken) instead.");
        }

        #endregion

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AuditLog>().ToTable("AuditLogs","audit");
        //    base.OnModelCreating(modelBuilder);
        //}

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <remarks>
        /// This method will automatically call <see cref="M:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.DetectChanges"/> to discover any changes
        ///                 to entity instances before saving to the underlying database. This can be disabled via
        ///                 <see cref="P:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled"/>.
        /// </remarks>
        /// <param name="userName">Username that will be used in the audit entry.</param>
        /// <returns>
        /// The number of state entries written to the underlying database.
        /// </returns>
        public virtual int SaveChanges(string userName)
        {
            var auditDateTime = DateTime.Now;
            var addedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            var modifiedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
            var deletedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).ToList();

            var auditLogs = AuditLogBuilder.GetAuditLogsForExistingEntities(userName, modifiedEntityEntries,
                deletedEntityEntries, ((IObjectContextAdapter)this).ObjectContext, auditDateTime);

            var result = base.SaveChanges();

            auditLogs.AddRange(AuditLogBuilder.GetAuditLogsForAddedEntities(userName, addedEntityEntries,
                ((IObjectContextAdapter) this).ObjectContext, auditDateTime));
            //auditLogs.

            Set<AuditLog>().AddRange(auditLogs);
            base.SaveChanges();
            return result;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will automatically call <see cref="M:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.DetectChanges"/> to discover any changes
        ///                     to entity instances before saving to the underlying database. This can be disabled via
        ///                     <see cref="P:Microsoft.Data.Entity.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled"/>.
        /// </para>
        /// <para>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///                     that any asynchronous operations have completed before calling another method on this context.
        /// </para>
        /// </remarks>
        /// <param name="userName">Username that will be used in the audit entry.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the
        ///                 number of state entries written to the underlying database.
        /// </returns>
        public virtual Task<int> SaveChangesAsync(string userName, CancellationToken cancellationToken)
        {
            //TODO: Implement this
            throw new NotImplementedException(
                $"Audit logic not implemented for SaveChangesAsync(string userName, CancellationToken cancellationToken) yet. Use SaveChanges(string userName) instead.");
            //return base.SaveChangesAsync(cancellationToken);
        }

      
    }
}
