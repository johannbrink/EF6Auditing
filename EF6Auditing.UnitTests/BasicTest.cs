using System;
using System.Data.Entity.Spatial;
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
                CreatedDate = DateTime.Now,
                Location = ConvertLatLonToDbGeography(-26.1167132503074, 28.0013997014612)
            };
            myDbContext.MyCustomers.Add(customer);
            myDbContext.SaveChanges("TestUser");
            customer.CustomerName = "Test2";
            customer.Location = ConvertLatLonToDbGeography(-24.1167132503074, 26.0013997014612);
            myDbContext.SaveChanges("TestUser");
        }

        public static DbGeography ConvertLatLonToDbGeography(double longitude, double latitude)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
        }
    }
}
