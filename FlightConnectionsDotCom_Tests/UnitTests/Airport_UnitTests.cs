using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Airport_UnitTests
    {
        [TestMethod]
        public void AirportIsInEurope()
        {
            Assert.IsTrue(new Airport("a", "a", "Bulgaria", "a", "a").AirportIsInEurope());
        }
        
        [TestMethod]
        public void AirportIsNotInEurope()
        {
            Assert.IsFalse(new Airport("a", "a", "Canada", "a", "a").AirportIsInEurope());
        }
        
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
