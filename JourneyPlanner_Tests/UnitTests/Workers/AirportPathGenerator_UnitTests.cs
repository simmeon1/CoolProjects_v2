using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class AirportPathGenerator_UnitTests
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
                { codeABZ, new HashSet<string>() { codeSOF, codeEDI, codeCIA } },
                { codeSOF, new HashSet<string>() { codeEDI, codeCIA, codeABZ } },
                { codeEDI, new HashSet<string>() { codeSOF, codeABZ, codeCIA } },
                { codeCIA, new HashSet<string>() { codeSOF, codeABZ, codeEDI } }
            };
            return new AirportPathGenerator(airportAndDestinationsList);
        }
        
        private static AirportPathGenerator GetPartialAirportAndDestinationsListWithLinks()
        {
            Dictionary<string, HashSet<string>> airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string>() { } },
                { codeSOF, new HashSet<string>() { codeEDI, codeCIA } },
                { codeEDI, new HashSet<string>() { codeSOF, codeCIA } },
                { codeCIA, new HashSet<string>() { codeSOF, codeEDI } }
            };

            Dictionary<string, HashSet<string>> airportLocalLinks = new()
            {
                { codeABZ, new HashSet<string>() { codeEDI } }
            };
            return new AirportPathGenerator(airportAndDestinationsList, airportLocalLinks);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsZeroResultsWhenMaxFlightsAreZero()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 0, 0, false);
            Assert.IsTrue(paths.Count == 0);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedOnePathWhenMaxFlightsAreOne()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 1, 0, false);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedTwoPathWhenMaxFlightsAreOne_WithBus()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 1, 0, false);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedOnePathWhenMaxFlightsAreOne_OnlyShortestFlightIsReturned()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2, 0, true);
            Assert.IsTrue(paths.Count == 1);
            VerifyAbzSofPath(paths);
        }

        [TestMethod]
        public void GetAirportConnections_MultipleOriginsAndTargets()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ, codeEDI }, new List<string>() { codeSOF, codeCIA }, 1, 0, false);
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
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2, 0, false);
            Assert.IsTrue(paths.Count == 3);
            VerifyAbzSofPath(paths);
            VerifyAbzCiaSofPath(paths);
            VerifyAbzEdiSofPath(paths);
        }

        [TestMethod]
        public void NoResultsWhenUsingPartialAirportDestinationsAndNoLocalLinks()
        {
            List<Path> paths = GetPartialAirportAndDestinationsListWithLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2, 0, false);
            Assert.IsTrue(paths.Count == 0);
        }
        
        [TestMethod]
        public void OneResultWhenUsingPartialAirportDestinationsAndLocalLinks()
        {
            List<Path> paths = GetPartialAirportAndDestinationsListWithLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 1, 1, false);
            Assert.IsTrue(paths.Count == 1);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-EDI-SOF"));
        }
        
        [TestMethod]
        public void TwoResultsWhenUsingPartialAirportDestinationsAndLocalLinks()
        {
            List<Path> paths = GetPartialAirportAndDestinationsListWithLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 2, 1, false);
            Assert.IsTrue(paths.Count == 2);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-EDI-SOF"));
            Assert.IsTrue(paths[1].ToString().Equals("ABZ-EDI-CIA-SOF"));
        }
        
        [TestMethod]
        public void ExpectedResultsWhenGettingAberdeenToVarnaPathsWithoutLocalLinks()
        {
            List<Path> paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 1, 0, false);
            Assert.IsTrue(paths.Count == 0);
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 1, 0, true);
            Assert.IsTrue(paths.Count == 0);

            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 2, 0, false);
            Assert.IsTrue(paths.Count == 1);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 2, 0, true);
            Assert.IsTrue(paths.Count == 1);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));

            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 3, 0, false);
            Assert.IsTrue(paths.Count == 1);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 3, 0, true);
            Assert.IsTrue(paths.Count == 1);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
        }
        
        [TestMethod]
        public void ExpectedResultsWhenGettingAberdeenToVarnaPathsWithLocalLinks()
        {
            List<Path> paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 1, 1, false);
            Assert.IsTrue(paths.Count == 0);
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 1, 1, true);
            Assert.IsTrue(paths.Count == 0);

            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 2, 1, false);
            Assert.IsTrue(paths.Count == 2);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            Assert.IsTrue(paths[1].ToString().Equals("ABZ-LTN-LHR-VAR"));

            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 2, 1, true);
            Assert.IsTrue(paths.Count == 2);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            Assert.IsTrue(paths[1].ToString().Equals("ABZ-LTN-LHR-VAR"));
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 3, 1, false);
            Assert.IsTrue(paths.Count == 3);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            Assert.IsTrue(paths[1].ToString().Equals("ABZ-LTN-LHR-VAR"));
            Assert.IsTrue(paths[2].ToString().Equals("ABZ-LTN-LHR-SOF-VAR"));
            
            paths = GetAberdeenToVarnaPathGeneratorWithLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeVAR }, 3, 1, true);
            Assert.IsTrue(paths.Count == 2);
            Assert.IsTrue(paths[0].ToString().Equals("ABZ-LTN-VAR"));
            Assert.IsTrue(paths[1].ToString().Equals("ABZ-LTN-LHR-VAR"));
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

            Dictionary<string, HashSet<string>> airportLocalLinks = new()
            {
                { codeLTN, new HashSet<string>() { codeLHR } }
            };
            AirportPathGenerator generator = new(airportAndDestinationsList, airportLocalLinks);
            return generator;
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedFivePathsWhenMaxFlightsAreThreeOrMore()
        {
            List<Path> paths = GetFullAirportAndDestinationsListWithoutLocalLinks().GeneratePaths(new List<string>() { codeABZ }, new List<string>() { codeSOF }, 10, 0, false);
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
