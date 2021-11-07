using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class AirportListFilterer_UnitTests
    {
        private const string codeABZ = "ABZ";
        private const string codeSOF = "SOF";
        private const string codeEDI = "EDI";
        private const string codeCIA = "CIA";
        List<Airport> airports;
        Dictionary<string, HashSet<string>> airportAndDestinationsList;
        AirportListFilterer airportListFilterer;
        private Airport ediAirport;

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
            airports = new();
            airports.Add(new Airport(codeABZ, "", "United Kingdom", "", ""));
            ediAirport = new Airport(codeEDI, "", "United Kingdom", "", "");
            airports.Add(ediAirport);
            airports.Add(new Airport(codeSOF, "", "Bulgaria", "", ""));
            airports.Add(new Airport(codeCIA, "", "Italy", "", ""));
            airportListFilterer = new(airports);
        }

        [TestMethod]
        public void AirportsRemainTheSame()
        {
            Dictionary<string, HashSet<string>> filteredAirports = airportListFilterer.FilterAirports(airportAndDestinationsList, null);
            Assert.IsTrue(filteredAirports.SerializeObject().Equals(airportAndDestinationsList.SerializeObject()));
        }
        
        [TestMethod]
        public void AirportsFiltered()
        {
            airports.Remove(ediAirport);
            Dictionary<string, HashSet<string>> filteredAirports = airportListFilterer.FilterAirports(airportAndDestinationsList, new UKBulgariaFilterer());
            Assert.IsTrue(filteredAirports.Count == 2);
            Assert.IsTrue(filteredAirports[codeABZ].Count == 1);
            Assert.IsTrue(filteredAirports[codeSOF].Count == 1);
            airports.Add(ediAirport);
        }
    }
}
