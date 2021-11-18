using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    }
}
