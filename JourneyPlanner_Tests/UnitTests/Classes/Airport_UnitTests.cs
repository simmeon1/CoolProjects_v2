using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class AirportUnitTests
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
    }
}
