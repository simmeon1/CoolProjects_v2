using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class UkBulgariaFiltererUnitTests
    {
        private readonly UKBulgariaFilterer filterer = new();
        [TestMethod]
        public void AirportIsUKOrBulgaria_Bulgaria()
        {
            Assert.IsTrue(filterer.AirportMeetsCondition(new Airport("a", "a", "Bulgaria", "a", "a")));
        }

        [TestMethod]
        public void AirportIsUKOrBulgaria_UK()
        {
            Assert.IsTrue(filterer.AirportMeetsCondition(new Airport("a", "a", "United Kingdom", "a", "a")));
        }
        
        [TestMethod]
        public void AirportIsUKOrBulgaria_Italy()
        {
            Assert.IsFalse(filterer.AirportMeetsCondition(new Airport("a", "a", "Italy", "a", "a")));
        }

        [TestMethod]
        public void AirportIsNotValid()
        {
            Assert.IsFalse(filterer.AirportMeetsCondition(null));
        }
    }
}
