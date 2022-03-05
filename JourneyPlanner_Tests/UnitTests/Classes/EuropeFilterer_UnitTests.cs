using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class EuropeFiltererUnitTests
    {
        private readonly EuropeFilterer filterer = new();
        [TestMethod]
        public void AirportIsInEurope_Bulgaria()
        {
            Assert.IsTrue(filterer.AirportMeetsCondition(new Airport("a", "a", "Bulgaria", "a", "a")));
        }

        [TestMethod]
        public void AirportIsInEurope_UK()
        {
            Assert.IsTrue(filterer.AirportMeetsCondition(new Airport("a", "a", "United Kingdom", "a", "a")));
        }
        
        [TestMethod]
        public void AirportIsNotInEurope_Canada()
        {
            Assert.IsFalse(filterer.AirportMeetsCondition(new Airport("a", "a", "Canada", "a", "a")));
        }

        [TestMethod]
        public void AirportIsNotValid()
        {
            Assert.IsFalse(filterer.AirportMeetsCondition(null));
        }
    }
}
