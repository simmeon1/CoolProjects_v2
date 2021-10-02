using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class FlightCollection_UnitTests
    {
        private readonly Flight flight1 = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
        private readonly Flight flight2 = new(new DateTime(2000, 10, 10, 20, 30, 40), new DateTime(2000, 11, 11, 21, 40, 50), "wizz", new TimeSpan(4, 0, 50), "EDI-VAR", 20);
        private List<Flight> entries;
        private FlightCollection collection;

        [TestInitialize]
        public void TestInitialize()
        {
            entries = new() { flight1, flight2 };
            collection = new(entries);
        }

        [TestMethod]
        public void GetEnumeratorIsCorrect()
        {
            Assert.IsTrue(collection.GetEnumerator().Equals(entries.GetEnumerator()));
        }

        [TestMethod]
        public void GetSetWorks()
        {
            collection[0] = flight2;
            Assert.IsTrue(collection[0] == flight2);
            collection[0] = flight1;
            Assert.IsTrue(collection[0] == flight1);
        }
    }
}
