using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class DirectPathUnitTests
    {
        [TestMethod]
        public void BuildsOK()
        {
            //For serializing
            DirectPath test = new();

            DirectPath directPath = new("ABZ", "EDI");
            Assert.IsTrue(directPath.GetStart().Equals("ABZ"));
            Assert.IsTrue(directPath.GetEnd().Equals("EDI"));
        }
    }
}
