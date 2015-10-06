using System;
using System.ComponentModel.DataAnnotations;
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

        [DoNotAudit]
        public DateTime CreatedDate { get; set; }
    }
}
