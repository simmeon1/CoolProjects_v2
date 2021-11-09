using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class NoFilterer_UnitTests
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
