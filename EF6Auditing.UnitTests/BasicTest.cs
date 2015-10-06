using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EF6Auditing.UnitTests
{
    [TestClass]
    public class BasicTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var myDbContext = new MyDbContext();
            myDbContext.Database.Initialize(true);
            var customer = new MyCustomer
            {
                CustomerName = "Test1",
                CreatedDate = DateTime.Now
            };
            myDbContext.MyCustomers.Add(customer);
            myDbContext.SaveChanges("TestUser");
            customer.CustomerName = "Test2";
            myDbContext.SaveChanges("TestUser");
        }
    }
}
