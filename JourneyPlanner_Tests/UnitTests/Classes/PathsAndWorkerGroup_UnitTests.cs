using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class PathsAndWorkerGroup_UnitTests
    {
        [TestMethod]
        public void ThrowsExceptionForPathWithCountOfPathEntriesDifferentThanTwo()
        {
            Assert.ThrowsException<Exception>(() => new PathsAndWorkerGroup(new List<Path>() { new Path(new List<string>() { "ABZ" }) }, ""));
        }
        
        [TestMethod]
        public void GroupConstructsCorrectly()
        {
            List<Path> paths = new() { new Path(new List<string>() { "ABZ", "EDI" }) };
            const string worker = "test";
            PathsAndWorkerGroup group = new(paths, worker);
            Assert.IsTrue(group.Paths.Equals(paths));
            Assert.IsTrue(group.Worker.Equals(worker));
        }
    }
}
