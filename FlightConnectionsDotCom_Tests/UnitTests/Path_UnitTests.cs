using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Path_UnitTests
    {
        private static readonly List<string> entries = new() { "ABZ", "EDI" };
        private Path path = new(entries);

        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(path.ToString().Equals("ABZ-EDI"));
        }
        
        [TestMethod]
        public void GetEnumeratorIsCorrect()
        {
            Assert.IsTrue(path.GetEnumerator().Equals(entries.GetEnumerator()));
        }
        
        [TestMethod]
        public void GetSetWorks()
        {
            path[0] = "LHR";
            Assert.IsTrue(path[0].Equals("LHR"));
            path[0] = "ABZ";
            Assert.IsTrue(path[0].Equals("ABZ"));
        }
    }
}
