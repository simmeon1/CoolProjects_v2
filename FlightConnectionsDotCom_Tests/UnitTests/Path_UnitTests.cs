using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Path_UnitTests
    {
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(new Path(new List<string>() { "ABZ", "EDI" }).ToString().Equals("ABZ-EDI"));
        }
    }
}
