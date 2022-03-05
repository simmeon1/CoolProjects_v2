using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class JourneyRetrieverDataUnitTests
    {
        [TestMethod]
        public void GroupConstructsCorrectly()
        {
            //For serializing
            JourneyRetrieverData test = new();

            List<DirectPath> paths = new() { new DirectPath("ABZ", "EDI") };
            Dictionary<string, string> transaltions = new();
            transaltions.Add("ABZ", "Aberdeen");
            JourneyRetrieverData data = new(paths, transaltions);
            Assert.IsTrue(data.DirectPaths.Equals(paths));
            Assert.IsTrue(data.Translations.Count == 1);
            Assert.IsTrue(data.GetTranslation("ABZ").Equals("Aberdeen"));
            Assert.IsTrue(data.GetTranslation("EDI").Equals("EDI"));
            Assert.IsTrue(data.ToString().Equals("1 direct paths, 1 translations."));
            data.RemovePath("ABZ-EDI");
            Assert.IsTrue(data.GetCountOfDirectPaths() == 0);
            Assert.IsTrue(data.GetKeyFromTranslation("Aberdeen").Equals("ABZ"));
            Assert.IsTrue(data.GetKeyFromTranslation("Edinburgh").Equals("Edinburgh"));

            
        }
    }
}
