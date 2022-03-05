using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class NoFiltererUnitTests
    {
        private readonly NoFilterer filterer = new();
        [TestMethod]
        public void AirportIsValid()
        {
            Assert.IsTrue(filterer.AirportMeetsCondition(new Airport("a", "a", "Bulgaria", "a", "a")));
        }
        
        [TestMethod]
        public void AirportIsNotValid()
        {
            Assert.IsFalse(filterer.AirportMeetsCondition(null));
        }
    }
}
