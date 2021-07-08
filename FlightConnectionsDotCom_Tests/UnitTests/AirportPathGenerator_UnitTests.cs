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

        Airport airport1;
        Airport airport2;
        Airport airport3;
        Airport airport4;
        ICollection<Airport> airportList;
        Dictionary<string, HashSet<string>> airportAndDestinationsList;
        AirportPathGenerator generator;

        [TestInitialize]
        public void TestInitialize()
        {
            airport1 = new(codeABZ, "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            airport2 = new(codeSOF, "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            airport3 = new(codeEDI, "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            airport4 = new(codeCIA, "Rome", "Italy", "Rome Airport", "linkD");
            airportList = new HashSet<Airport>() { airport1, airport2, airport3, airport4 };
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
            Assert.IsTrue(paths[0].Count == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeSOF));
        }

        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedThreePathsWhenMaxFlightsAreTwo()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 2);
            Assert.IsTrue(paths.Count == 3);
            Assert.IsTrue(paths[0].Count == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeSOF));
            Assert.IsTrue(paths[1].Count == 3);
            Assert.IsTrue(paths[1][0].Equals(codeABZ));
            Assert.IsTrue(paths[1][1].Equals(codeEDI));
            Assert.IsTrue(paths[1][2].Equals(codeSOF));
            Assert.IsTrue(paths[2].Count == 3);
            Assert.IsTrue(paths[2][0].Equals(codeABZ));
            Assert.IsTrue(paths[2][1].Equals(codeCIA));
            Assert.IsTrue(paths[2][2].Equals(codeSOF));
        }
        
        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedThreePathsWhenMaxFlightsAreThree()
        {
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF, 3);
            Assert.IsTrue(paths.Count == 5);
            //Assert.IsTrue(paths[0].Count == 5);
            //Assert.IsTrue(paths[0][0].Equals(codeABZ));
            //Assert.IsTrue(paths[0][1].Equals(codeSOF));
            //Assert.IsTrue(paths[1].Count == 3);
            //Assert.IsTrue(paths[1][0].Equals(codeABZ));
            //Assert.IsTrue(paths[1][1].Equals(codeEDI));
            //Assert.IsTrue(paths[1][2].Equals(codeSOF));
            //Assert.IsTrue(paths[2].Count == 3);
            //Assert.IsTrue(paths[2][0].Equals(codeABZ));
            //Assert.IsTrue(paths[2][1].Equals(codeCIA));
            //Assert.IsTrue(paths[2][2].Equals(codeSOF));
        }
    }
}
