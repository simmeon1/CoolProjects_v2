using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class PathsAndWorkerGrouper_UnitTests
    {        
        [TestMethod]
        public void test()
        {
            Path path = new(new List<string>() { "ABZ", "EDI", "GLA", "LTN" });
            List<Path> paths = new() { path };
            const string worker1 = "worker1";
            const string worker2 = "worker2";
            const string worker3 = PathsAndWorkerGrouper.DEFAULT_WORKER;
            PathsAndWorkerGroup group1 = new(new List<Path>() { new Path(new List<string>() { "ABZ", "EDI" }) }, worker1);
            PathsAndWorkerGroup group2 = new(new List<Path>() { new Path(new List<string>() { "EDI", "GLA" }) }, worker2);
            List<PathsAndWorkerGroup> groups = new() { group1, group2 };
            PathsAndWorkerGrouper grouper = new();
            Dictionary<string, List<Path>> grouperGroups = grouper.GetGroups(paths, groups);
            Assert.IsTrue(grouperGroups.Count == 3);
            Assert.IsTrue(grouperGroups.ContainsKey(worker1));
            Assert.IsTrue(grouperGroups[worker1].Count == 1);
            Assert.IsTrue(grouperGroups[worker1][0].ToString().Equals("ABZ-EDI"));
            Assert.IsTrue(grouperGroups.ContainsKey(worker2));
            Assert.IsTrue(grouperGroups[worker2].Count == 1);
            Assert.IsTrue(grouperGroups[worker2][0].ToString().Equals("EDI-GLA"));
            Assert.IsTrue(grouperGroups.ContainsKey(worker3));
            Assert.IsTrue(grouperGroups[worker3].Count == 1);
            Assert.IsTrue(grouperGroups[worker3][0].ToString().Equals("GLA-LTN"));
        }
    }
}
