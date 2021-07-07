using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class AirportPathGenerator_UnitTests
    {
        [TestMethod]
        public void GetAirportConnections_ReturnsExpectedValues()
        {
            const string codeABZ = "ABZ";
            const string codeSOF = "SOF";
            const string codeEDI = "EDI";
            const string codeCIA = "CIA";
            Airport airport1 = new(codeABZ, "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new(codeSOF, "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new(codeEDI, "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            Airport airport4 = new(codeCIA, "Rome", "Italy", "Rome Airport", "linkD");
            ICollection<Airport> airportList = new HashSet<Airport>() { airport1, airport2, airport3, airport4 };

            Dictionary<string, HashSet<string>> airportAndDestinationsList = new()
            {
                { codeABZ, new HashSet<string>() { codeSOF } },
                { codeSOF, new HashSet<string>() { codeEDI } },
                { codeEDI, new HashSet<string>() { codeSOF } }
            };
            AirportPathGenerator generator = new(airportList, airportAndDestinationsList);
            List<List<string>> paths = generator.GeneratePaths(codeABZ, codeSOF);
            Assert.IsTrue(paths.Count == 2);
            Assert.IsTrue(paths[0].Count == 2);
            Assert.IsTrue(paths[0][0].Equals(codeABZ));
            Assert.IsTrue(paths[0][1].Equals(codeSOF));
            Assert.IsTrue(paths[1].Count == 4);
            Assert.IsTrue(paths[1][0].Equals(codeABZ));
            Assert.IsTrue(paths[1][1].Equals(codeSOF));
            Assert.IsTrue(paths[1][2].Equals(codeEDI));
            Assert.IsTrue(paths[1][3].Equals(codeSOF));
        }
    }
}
