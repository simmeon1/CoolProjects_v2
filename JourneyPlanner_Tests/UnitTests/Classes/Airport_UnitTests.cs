using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class Airport_UnitTests
    {        
        [TestMethod]
        public void AirportsHaveEqualHashCode()
        {
            Assert.IsTrue(new Airport("a", "a", "Bulgaria", "a", "a").GetHashCode() == new Airport("a", "a", "Bulgaria", "a", "a").GetHashCode());
        }
        
        [TestMethod]
        public void AirportsAreEqual()
        {
            Assert.IsTrue(new Airport("a", "a", "Bulgaria", "a", "a").Equals(new Airport("a", "a", "Bulgaria", "a", "a")));
        }
        
        [TestMethod]
        public void AirportsAreNotEqual()
        {
            Assert.IsFalse(new Airport("a", "a", "Bulgaria", "a", "a").Equals(1));
        }
        
        [TestMethod]
        public void asd()
        {
            List<DirectPath> paths = new();
            paths.Add(new DirectPath("LTN", "LHR"));
            paths.Add(new DirectPath("LTN", "LGW"));
            paths.Add(new DirectPath("LTN", "STN"));
            paths.Add(new DirectPath("LHR", "LTN"));
            paths.Add(new DirectPath("LHR", "LGW"));
            paths.Add(new DirectPath("LHR", "STN"));
            paths.Add(new DirectPath("LGW", "LTN"));
            paths.Add(new DirectPath("LGW", "LHR"));
            paths.Add(new DirectPath("LGW", "STN"));
            paths.Add(new DirectPath("STN", "LTN"));
            paths.Add(new DirectPath("STN", "LHR"));
            paths.Add(new DirectPath("STN", "LGW"));
            Dictionary<string, string> translations = new();
            translations.Add("LTN", "LTN");
            JourneyRetrieverData data = new(paths, translations);

            Dictionary<string, JourneyRetrieverData> dict = new();
            dict.Add("NationalExpressWorker", data);

            var str = dict.SerializeObject(Newtonsoft.Json.Formatting.Indented);
        }
    }
}
