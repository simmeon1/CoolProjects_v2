using System.Collections.Generic;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class PathUnitTests
    {
        private static readonly List<string> entries = new() { "ABZ", "EDI" };
        private readonly Path path = new(entries);
        private readonly List<string> bigPath = new() { "ABZ", "LTN", "SOF" };

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
            Assert.IsTrue(new Path(bigPath).GetSummarisedPath().Equals("ABZ-SOF"));
        }
        
        [TestMethod]
        public void GetDirectPathsReturnsExpectedPaths()
        {
            List<DirectPath> directPaths = new Path(bigPath).GetDirectPaths();
            Assert.IsTrue(directPaths.Count == 2);
            Assert.IsTrue(directPaths[0].GetStart().Equals("ABZ"));
            Assert.IsTrue(directPaths[0].GetEnd().Equals("LTN"));
            Assert.IsTrue(directPaths[1].GetStart().Equals("LTN"));
            Assert.IsTrue(directPaths[1].GetEnd().Equals("SOF"));
        }
        
        [TestMethod]
        public void GetDirectPathsReturnsExpectedNothing()
        {
            Assert.IsTrue(new Path(new List<string>() { "ABZ" }).GetDirectPaths().Count == 0);
        }
    }
}
