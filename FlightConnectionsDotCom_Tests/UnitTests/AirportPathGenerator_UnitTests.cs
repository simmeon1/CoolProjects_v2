using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class AirportPathGenerator_UnitTests
    {
        private const string codeABZ = "ABZ";
        private const string codeSOF = "SOF";
        private const string codeEDI = "EDI";
        private const string codeCIA = "CIA";
        Dictionary<string, HashSet<string>> airportAndDestinationsList;
        AirportPathGenerator generator;

        [TestInitialize]
        public void TestInitialize()
        {
            airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string>() { codeSOF, codeEDI, codeCIA } },
                { codeSOF, new HashSet<string>() { codeEDI, codeCIA, codeABZ } },
                { codeEDI, new HashSet<string>() { codeSOF, codeABZ, codeCIA } },
                { codeCIA, new HashSet<string>() { codeSOF, codeABZ, codeEDI } }
            };
            generator = new(airportAndDestinationsList);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsZeroResultsWhenMaxFlightsAreZero()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 0);
            Assert.IsTrue(paths.Count == 0);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedOnePathWhenMaxFlightsAreOne()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 1);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedThreePathsWhenMaxFlightsAreTwo()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 2);
            Assert.IsTrue(paths.Count == 3);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedFivePathsWhenMaxFlightsAreThreeOrMore()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 10);
            Assert.IsTrue(paths.Count == 5);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
            Assert.IsTrue(paths[3].Count == 4);
            Assert.IsTrue(paths[3][0].Equals(codeABZ));
            Assert.IsTrue(paths[3][1].Equals(codeCIA));
            Assert.IsTrue(paths[3][2].Equals(codeEDI));
            Assert.IsTrue(paths[3][3].Equals(codeSOF));
            Assert.IsTrue(paths[4].Count == 4);
            Assert.IsTrue(paths[4][0].Equals(codeABZ));
            Assert.IsTrue(paths[4][1].Equals(codeEDI));
            Assert.IsTrue(paths[4][2].Equals(codeCIA));
            Assert.IsTrue(paths[4][3].Equals(codeSOF));
        }

        private static void VerifyAbzSofPath(List<List<string>> paths)
        {
            Assert.IsTrue(paths[0].Count == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeSOF));
        }
        private static void VerifyAbzCiaSofPath(List<List<string>> paths)
        {
            Assert.IsTrue(paths[1].Count == 3);
            Assert.IsTrue(paths[1][0].Equals(codeABZ));
            Assert.IsTrue(paths[1][1].Equals(codeCIA));
            Assert.IsTrue(paths[1][2].Equals(codeSOF));
        }

        private static void VerifyAbzEdiSofPath(List<List<string>> paths)
        {
            Assert.IsTrue(paths[2].Count == 3);
            Assert.IsTrue(paths[2][0].Equals(codeABZ));
            Assert.IsTrue(paths[2][1].Equals(codeEDI));
            Assert.IsTrue(paths[2][2].Equals(codeSOF));
        }
    }
}
