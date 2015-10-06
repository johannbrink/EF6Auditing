using System.Data.Entity;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EF6Auditing.UnitTests
{
    internal class MyDbContext : AuditingDbContext
    {
        public MyDbContext() : base("DefaultConnectionString")
        {
            
        }

   
        public DbSet<MyCustomer> MyCustomers { get; set; }

    }
}