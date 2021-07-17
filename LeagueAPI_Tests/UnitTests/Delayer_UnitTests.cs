using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class Delayer_UnitTests
    {
        [TestMethod]
        public async Task Delay_Working()
        {
            Delayer delayer = new();
            await delayer.Delay(1);
            Assert.IsTrue(true);
        }
    }
}
