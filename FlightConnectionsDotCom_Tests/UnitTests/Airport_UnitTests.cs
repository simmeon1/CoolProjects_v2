using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
    }
}
