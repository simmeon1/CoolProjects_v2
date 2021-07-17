using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class WinLossData_UnitTests
    {
        [TestMethod]
        public void WinLossData_GettersAndSettersWorking()
        {
            WinLossData data = new();
            Assert.IsTrue(data.GetWins() == 0);
            Assert.IsTrue(data.GetLosses() == 0);
            data.AddWin();
            data.AddLoss();
            data.AddLoss();
            Assert.IsTrue(data.GetWins() == 1);
            Assert.IsTrue(data.GetLosses() == 2);
            Assert.IsTrue(data.GetTotal() == 3);
        }

        [TestMethod]
        public void WinLossData_GetWinRate_IsCorrect()
        {
            WinLossData data = new();
            Assert.IsTrue(data.GetWinRate() == 0);
            data.AddWin();
            Assert.IsTrue(data.GetWinRate() == 100);
            data.AddLoss();
            Assert.IsTrue(data.GetWinRate() == 50);
            data.AddLoss();
            Assert.IsTrue(Math.Round(data.GetWinRate()) == 33);
            Assert.IsTrue(data.GetString().Length > 0);
        }
    }
}
