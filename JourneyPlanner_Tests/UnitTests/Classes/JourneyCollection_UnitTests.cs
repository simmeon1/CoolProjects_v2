using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class JourneyCollection_UnitTests
    {
        private readonly Journey journey1 = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
        private readonly Journey journey2 = new(new DateTime(2000, 10, 10, 20, 30, 40), new DateTime(2000, 11, 11, 21, 40, 50), "wizz", new TimeSpan(4, 0, 50), "EDI-VAR", 20);
        private List<Journey> entries;
        private JourneyCollection collection;

        [TestInitialize]
        public void TestInitialize()
        {
            entries = new() { journey1, journey2 };
            collection = new(entries);
        }

        [TestMethod]
        public void GetSetWorks()
        {
            collection[0] = journey2;
            Assert.IsTrue(collection[0] == journey2);
            collection[0] = journey1;
            Assert.IsTrue(collection[0] == journey1);
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
            JourneyCollection collection2 = json.DeserializeObject<JourneyCollection>();
            Assert.IsTrue(collection.GetCount() == collection2.GetCount());
        }
    }
}
