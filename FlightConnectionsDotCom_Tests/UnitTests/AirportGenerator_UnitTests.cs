using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnectionsDotCom_Tests_UnitTests
{
    [TestClass]
    public class AirportGenerator_UnitTests
    {
        [TestMethod]
        public void GetAllPossiblePermutationsOfLetters_ReturnsAllPermutationsOfABC()
        {
            AirportGenerator airportGenerator = new();
            List<string> result = airportGenerator.GetAllPossiblePermutationsOfLetters("ABC");
            Assert.IsTrue(result.Distinct().Count() == 27);
            Assert.IsTrue(result[0].Equals("AAA"));
            Assert.IsTrue(result[1].Equals("AAB"));
            Assert.IsTrue(result[2].Equals("AAC"));
            Assert.IsTrue(result[3].Equals("ABA"));
            Assert.IsTrue(result[4].Equals("ABB"));
            Assert.IsTrue(result[5].Equals("ABC"));
            Assert.IsTrue(result[6].Equals("ACA"));
            Assert.IsTrue(result[7].Equals("ACB"));
            Assert.IsTrue(result[8].Equals("ACC"));
            Assert.IsTrue(result[9].Equals("BAA"));
            Assert.IsTrue(result[10].Equals("BAB"));
            Assert.IsTrue(result[11].Equals("BAC"));
            Assert.IsTrue(result[12].Equals("BBA"));
            Assert.IsTrue(result[13].Equals("BBB"));
            Assert.IsTrue(result[14].Equals("BBC"));
            Assert.IsTrue(result[15].Equals("BCA"));
            Assert.IsTrue(result[16].Equals("BCB"));
            Assert.IsTrue(result[17].Equals("BCC"));
            Assert.IsTrue(result[18].Equals("CAA"));
            Assert.IsTrue(result[19].Equals("CAB"));
            Assert.IsTrue(result[20].Equals("CAC"));
            Assert.IsTrue(result[21].Equals("CBA"));
            Assert.IsTrue(result[22].Equals("CBB"));
            Assert.IsTrue(result[23].Equals("CBC"));
            Assert.IsTrue(result[24].Equals("CCA"));
            Assert.IsTrue(result[25].Equals("CCB"));
            Assert.IsTrue(result[26].Equals("CCC"));
        }

        [TestMethod]
        public void GetAllPossiblePermutationsOfLetters_Returns17576CombinationsFromEnglishAlphabet()
        {
            AirportGenerator airportGenerator = new();
            List<string> result = airportGenerator.GetAllPossiblePermutationsOfLetters("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            Assert.IsTrue(result.Distinct().Count() == 17576);
        }
    }
}
