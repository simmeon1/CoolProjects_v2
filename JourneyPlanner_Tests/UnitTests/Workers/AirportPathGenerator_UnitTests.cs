using System.Collections.Generic;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JourneyPlanner_Tests.UnitTests.Workers
{
    [TestClass]
    public class AirportPathGeneratorUnitTests
    {
        private const string codeABZ = "ABZ";
        private const string codeSOF = "SOF";
        private const string codeEDI = "EDI";
        private const string codeGLA = "GLA";
        private const string codeCIA = "CIA";
        private const string codeLTN = "LTN";
        private const string codeLHR = "LHR";
        private const string codeVAR = "VAR";

        private static AirportPathGenerator GetFullAirportAndDestinationsListWithoutLocalLinks()
        {
            Dictionary<string, HashSet<string>> airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string> { codeSOF, codeEDI, codeCIA } },
                { codeSOF, new HashSet<string> { codeEDI, codeCIA, codeABZ } },
                { codeEDI, new HashSet<string> { codeSOF, codeABZ, codeCIA } },
                { codeCIA, new HashSet<string> { codeSOF, codeABZ, codeEDI } }
            };
            return new AirportPathGenerator(new Mock<ILogger>().Object, airportAndDestinationsList);
        }
        
        private static AirportPathGenerator GetPartialAirportAndDestinationsListWithLinks()
        {
            Dictionary<string, HashSet<string>> airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string>()},
                { codeSOF, new HashSet<string> { codeEDI, codeCIA } },
                { codeEDI, new HashSet<string> { codeSOF, codeCIA } },
                { codeCIA, new HashSet<string> { codeSOF, codeEDI } }
            };

            return new AirportPathGenerator(new Mock<ILogger>().Object, airportAndDestinationsList);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsZeroResultsWhenMaxFlightsAreZero()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 0);
            Assert.IsTrue(paths.Count == 0);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedOnePathWhenMaxFlightsAreOne()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 1);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }
        
        [TestMethod]
        public void GetAirportConnections_MultipleOriginsAndTargets()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ, codeEDI }, new List<string>() { codeSOF, codeCIA }, 1);
            Assert.IsTrue(paths.Count == 4);
            Assert.IsTrue(paths[0].Count() == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeCIA));
            Assert.IsTrue(paths[1].Count() == 2);
            Assert.IsTrue(paths[1][0].Equals(codeABZ));
            Assert.IsTrue(paths[1][1].Equals(codeSOF));
            Assert.IsTrue(paths[2].Count() == 2);
            Assert.IsTrue(paths[2][0].Equals(codeEDI));
            Assert.IsTrue(paths[2][1].Equals(codeCIA));
            Assert.IsTrue(paths[3].Count() == 2);
            Assert.IsTrue(paths[3][0].Equals(codeEDI));
            Assert.IsTrue(paths[3][1].Equals(codeSOF));
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedThreePathsWhenMaxFlightsAreTwo()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2);
            Assert.IsTrue(paths.Count == 3);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
        }

        [TestMethod]
        public void NoResultsWhenUsingPartialAirportDestinationsAndNoLocalLinks()
        {
            List<Path> paths = GetPartialAirportAndDestinationsListWithLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2);
            Assert.IsTrue(paths.Count == 0);
        }
        
        private static AirportPathGenerator GetAberdeenToVarnaPathGeneratorWithLocalLinks()
        {
            Dictionary<string, HashSet<string>> airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string>() { codeLTN } },
                { codeLTN, new HashSet<string>() { codeVAR } },
                { codeLHR, new HashSet<string>() { codeVAR, codeSOF } },
                { codeSOF, new HashSet<string>() { codeVAR } },
                { codeVAR, new HashSet<string>() { codeSOF, codeLHR } }
            };

            AirportPathGenerator generator = new(new Mock<ILogger>().Object, airportAndDestinationsList);
            return generator;
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedFivePathsWhenMaxFlightsAreThreeOrMore()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 10);
            Assert.IsTrue(paths.Count == 5);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
            Assert.IsTrue(paths[3].Count() == 4);
            Assert.IsTrue(paths[3][0].Equals(codeABZ));
            Assert.IsTrue(paths[3][1].Equals(codeCIA));
            Assert.IsTrue(paths[3][2].Equals(codeEDI));
            Assert.IsTrue(paths[3][3].Equals(codeSOF));
            Assert.IsTrue(paths[4].Count() == 4);
            Assert.IsTrue(paths[4][0].Equals(codeABZ));
            Assert.IsTrue(paths[4][1].Equals(codeEDI));
            Assert.IsTrue(paths[4][2].Equals(codeCIA));
            Assert.IsTrue(paths[4][3].Equals(codeSOF));
        }

        private static void VerifyAbzSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[0].Count() == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeSOF));
        }
        private static void VerifyAbzCiaSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[1].Count() == 3);
            Assert.IsTrue(paths[1][0].Equals(codeABZ));
            Assert.IsTrue(paths[1][1].Equals(codeCIA));
            Assert.IsTrue(paths[1][2].Equals(codeSOF));
        }

        private static void VerifyAbzEdiSofPath(List<Path> paths)
        {
            Assert.IsTrue(paths[2].Count() == 3);
            Assert.IsTrue(paths[2][0].Equals(codeABZ));
            Assert.IsTrue(paths[2][1].Equals(codeEDI));
            Assert.IsTrue(paths[2][2].Equals(codeSOF));
        }
    }
}
