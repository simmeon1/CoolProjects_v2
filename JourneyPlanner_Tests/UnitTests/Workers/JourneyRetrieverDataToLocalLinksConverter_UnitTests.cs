using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class JourneyRetrieverDataToLocalLinksConverter_UnitTests
    {
        [TestMethod]
        public void ExpectedResults()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();
            existingData.Add("test", new JourneyRetrieverData(new List<DirectPath>() { new DirectPath("ABZ", "LTN"), new DirectPath("LTN", "ABZ") }));
            existingData.Add("test2", new JourneyRetrieverData(new List<DirectPath>() { new DirectPath("ABZ", "EDI"), new DirectPath("EDI", "ABZ") }));
            existingData.Add("test3", new JourneyRetrieverData(new List<DirectPath>() { new DirectPath("ABZ", "EDI"), new DirectPath("EDI", "ABZ") }));

            JourneyRetrieverDataToLocalLinksConverter converter = new();
            Dictionary<string, HashSet<string>> result = converter.DoConversion(existingData);
            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result.ContainsKey("ABZ"));
            Assert.IsTrue(result.ContainsKey("EDI"));
            Assert.IsTrue(result.ContainsKey("LTN"));
            Assert.IsTrue(result["ABZ"].Count == 2);
            Assert.IsTrue(result["LTN"].Count == 1);
            Assert.IsTrue(result["EDI"].Count == 1);
            Assert.IsTrue(result["ABZ"].Contains("EDI"));
            Assert.IsTrue(result["ABZ"].Contains("LTN"));
            Assert.IsTrue(result["LTN"].Contains("ABZ"));
            Assert.IsTrue(result["EDI"].Contains("ABZ"));
        }
    }
}
