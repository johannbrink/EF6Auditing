using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EF6Auditing.UnitTests
{
    class MyCustomer
    {
        [Key]
        [DoNotAudit]
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public DbGeography Location { get; set; }

        [DoNotAudit]
        public DateTime CreatedDate { get; set; }
    }
}
