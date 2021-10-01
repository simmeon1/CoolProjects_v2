using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Flight_UnitTests
    {        
        [TestMethod]
        public void GetFullStringIsCorrect()
        {
            //TimeSpan
            Assert.IsTrue(new Airport("a", "a", "Bulgaria", "a", "a").GetHashCode() == new Airport("a", "a", "Bulgaria", "a", "a").GetHashCode());
        }
    }
}
