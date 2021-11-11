using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class JourneyRetrieverData_UnitTests
    {
        [TestMethod]
        public void GroupConstructsCorrectly()
        {
            List<DirectPath> paths = new() { new DirectPath("ABZ", "EDI") };
            Dictionary<string, string> transaltions = new();
            transaltions.Add("ABZ", "Aberdeen");
            JourneyRetrieverData data = new(paths, transaltions);
            Assert.IsTrue(data.DirectPaths.Equals(paths));
            Assert.IsTrue(data.Translations.Count == 1);
            Assert.IsTrue(data.GetTranslation("ABZ").Equals("Aberdeen"));
            Assert.IsTrue(data.GetTranslation("EDI").Equals("EDI"));
            Assert.IsTrue(data.ToString().Equals("1 direct paths, 1 translations."));
        }
    }
}
