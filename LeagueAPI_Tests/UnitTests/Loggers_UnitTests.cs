using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class Loggers_UnitTests
    {
        [TestMethod]
        public void LoggerDebug_Test()
        {
            Logger_Debug logger = new();
            logger.Log("1");
            Assert.IsTrue(logger.Contains("1"));
            Assert.IsTrue(!logger.Contains("2"));
        }
        
        [TestMethod]
        public void LoggerConsole_Test()
        {
            Logger_Console logger = new();
            logger.Log("1");
            Assert.IsTrue(logger.Contains("1"));
            Assert.IsTrue(!logger.Contains("2"));
        }
    }
}
