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
            List<Path> paths = generator.GeneratePaths(new List<string>(){ codeABZ }, new List<string>(){ codeSOF }, 0);
            Assert.IsTrue(paths.Count == 0);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedOnePathWhenMaxFlightsAreOne()
        {
            List<Path> paths = generator.GeneratePaths(new List<string>(){ codeABZ }, new List<string>(){ codeSOF }, 1);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_MultipleOriginsAndTargets()
        {
            List<Path> paths = generator.GeneratePaths(new List<string>(){ codeABZ, codeEDI }, new List<string>(){ codeSOF, codeCIA }, 1);
            Assert.IsTrue(paths.Count == 4);
            Assert.IsTrue(paths[0].Entries.Count == 2);
            Assert.IsTrue(paths[0].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[0].Entries[1].Equals(codeCIA));
            Assert.IsTrue(paths[1].Entries.Count == 2);
            Assert.IsTrue(paths[1].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[1].Entries[1].Equals(codeSOF));
            Assert.IsTrue(paths[2].Entries.Count == 2);
            Assert.IsTrue(paths[2].Entries[0].Equals(codeEDI));
            Assert.IsTrue(paths[2].Entries[1].Equals(codeCIA));
            Assert.IsTrue(paths[3].Entries.Count == 2);
            Assert.IsTrue(paths[3].Entries[0].Equals(codeEDI));
            Assert.IsTrue(paths[3].Entries[1].Equals(codeSOF));
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedThreePathsWhenMaxFlightsAreTwo()
        {
            List<Path> paths = generator.GeneratePaths(new List<string>(){ codeABZ }, new List<string>(){ codeSOF }, 2);
            Assert.IsTrue(paths.Count == 3);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedFivePathsWhenMaxFlightsAreThreeOrMore()
        {
            List<Path> paths = generator.GeneratePaths(new List<string>(){ codeABZ }, new List<string>(){ codeSOF }, 10);
            Assert.IsTrue(paths.Count == 5);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
            Assert.IsTrue(paths[3].Entries.Count == 4);
            Assert.IsTrue(paths[3].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[3].Entries[1].Equals(codeCIA));
            Assert.IsTrue(paths[3].Entries[2].Equals(codeEDI));
            Assert.IsTrue(paths[3].Entries[3].Equals(codeSOF));
            Assert.IsTrue(paths[4].Entries.Count == 4);
            Assert.IsTrue(paths[4].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[4].Entries[1].Equals(codeEDI));
            Assert.IsTrue(paths[4].Entries[2].Equals(codeCIA));
            Assert.IsTrue(paths[4].Entries[3].Equals(codeSOF));
        }

        private static void VerifyAbzSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[0].Entries.Count == 2);
            Assert.IsTrue(paths[0].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[0].Entries[1].Equals(codeSOF));
        }
        private static void VerifyAbzCiaSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[1].Entries.Count == 3);
            Assert.IsTrue(paths[1].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[1].Entries[1].Equals(codeCIA));
            Assert.IsTrue(paths[1].Entries[2].Equals(codeSOF));
        }

        private static void VerifyAbzEdiSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[2].Entries.Count == 3);
            Assert.IsTrue(paths[2].Entries[0].Equals(codeABZ));
            Assert.IsTrue(paths[2].Entries[1].Equals(codeEDI));
            Assert.IsTrue(paths[2].Entries[2].Equals(codeSOF));
        }
    }
}
