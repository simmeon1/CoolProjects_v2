using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class Path_UnitTests
    {
        private static readonly List<string> entries = new() { "ABZ", "EDI" };
        private readonly Path path = new(entries);

        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(path.ToString().Equals("ABZ-EDI"));
        }

        [TestMethod]
        public void SerialiseDeserialize()
        {
            string json = path.SerializeObject();
            Path path2 = json.DeserializeObject<Path>();
            Assert.IsTrue(path.ToString().Equals(path2.ToString()));
        }
        
        [TestMethod]
        public void GetSetWorks()
        {
            path[0] = "LHR";
            Assert.IsTrue(path[0].Equals("LHR"));
            path[0] = "ABZ";
            Assert.IsTrue(path[0].Equals("ABZ"));
        }
        
        [TestMethod]
        public void GetSummarisedPath()
        {
            Assert.IsTrue(new Path(new List<string>() { "ABZ", "LTN", "SOF" }).GetSummarisedPath().Equals("ABZ-SOF"));
        }
    }
}
