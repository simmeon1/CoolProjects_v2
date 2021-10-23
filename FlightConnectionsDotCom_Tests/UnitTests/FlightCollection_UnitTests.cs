using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class FlightCollection_UnitTests
    {
        private readonly Airport airport1 = new("ABZ", country: "UK");
        private readonly Airport airport2 = new("EDI", country: "UK");
        private readonly Airport airport3 = new("VAR", country: "BG");
        private Flight flight1;
        private Flight flight2;
        private List<Flight> entries;
        private FlightCollection collection;

        [TestInitialize]
        public void TestInitialize()
        {
            flight1 = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), airport1, airport2, 25);
            flight2 = new(new DateTime(2000, 10, 10, 20, 30, 40), new DateTime(2000, 11, 11, 21, 40, 50), "wizz", new TimeSpan(4, 0, 50), airport2, airport3, 20);
            entries = new() { flight1, flight2 };
            collection = new(entries);
        }

        [TestMethod]
        public void GetSetWorks()
        {
            collection[0] = flight2;
            Assert.IsTrue(collection[0] == flight2);
            collection[0] = flight1;
            Assert.IsTrue(collection[0] == flight1);
        }

        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(collection.ToString().Equals("2 flights"));
        }

        [TestMethod]
        public void SerialiseDeserialize()
        {
            string json = collection.SerializeObject();
            FlightCollection collection2 = json.DeserializeObject<FlightCollection>();
            Assert.IsTrue(collection.Count() == collection2.Count());
        }
    }
}
