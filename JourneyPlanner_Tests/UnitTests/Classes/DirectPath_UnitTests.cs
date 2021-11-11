using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class DirectPath_UnitTests
    {
        [TestMethod]
        public void BuildsOK()
        {
            DirectPath directPath = new("ABZ", "EDI");
            Assert.IsTrue(directPath.GetStart().Equals("ABZ"));
            Assert.IsTrue(directPath.GetEnd().Equals("EDI"));
        }
    }
}
