using System;
using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class ParametersUnitTests
    {
        private Parameters p;
        
        [TestInitialize]
        public void Initialise()
        {
            p = new Parameters();
        }
        
        [TestMethod]
        public void OriginsGetSetWorks()
        {
            List<string> list = new();
            p.Origins = list;
            Assert.IsTrue(p.Origins == list);
        }

        [TestMethod]
        public void DestinationsGetSetWorks()
        {
            List<string> list = new();
            p.Destinations = list;
            Assert.IsTrue(p.Destinations == list);
        }

        [TestMethod]
        public void MaxFlightsGetSetWorks()
        {
            p.MaxFlights = 2;
            Assert.IsTrue(p.MaxFlights == 2);
        }

        [TestMethod]
        public void TransportFromHomeCostGetSetWorks()
        {
            p.TransportFromHomeCost = 2;
            Assert.IsTrue(p.TransportFromHomeCost == 2);
        }
        
        [TestMethod]
        public void ExtraCostPerFlightGetSetWorks()
        {
            p.ExtraCostPerFlight = 2;
            Assert.IsTrue(p.ExtraCostPerFlight == 2);
        }
        
        [TestMethod]
        public void MaxLocalLinksGetSetWorks()
        {
            p.MaxLocalLinks = 2;
            Assert.IsTrue(p.MaxLocalLinks == 2);
        }

        [TestMethod]
        public void DateFromGetSetWorks()
        {
            DateTime date = new();
            p.DateFrom = date;
            Assert.IsTrue(p.DateFrom == date);
        }

        [TestMethod]
        public void DateToGetSetWorks()
        {
            DateTime date = new();
            p.DateTo = date;
            Assert.IsTrue(p.DateTo == date);
        }

        [TestMethod]
        public void IncludeCheckedInBaggageGetSetWorks()
        {
            p.IncludeCheckedInBaggage = true;
            Assert.IsTrue(p.IncludeCheckedInBaggage);
        }

        [TestMethod]
        public void AirportListFileGetSetWorks()
        {
            p.AirportListFile = "asd";
            Assert.IsTrue(p.AirportListFile.Equals("asd"));
        }

        [TestMethod]
        public void AirportDestinationsFileGetSetWorks()
        {
            p.AirportDestinationsFile = "asd";
            Assert.IsTrue(p.AirportDestinationsFile.Equals("asd"));
        }

        [TestMethod]
        public void ProgressFileGetSetWorks()
        {
            p.ProgressFile = "asd";
            Assert.IsTrue(p.ProgressFile.Equals("asd"));
        }

        [TestMethod]
        public void WorkerSetupFileGetSetWorks()
        {
            p.WorkerSetupFile = "asd";
            Assert.IsTrue(p.WorkerSetupFile.Equals("asd"));
        }

        [TestMethod]
        public void SkipUndoableJourneysGetSetWorks()
        {
            p.SkipUndoableJourneys = true;
            Assert.IsTrue(p.SkipUndoableJourneys);
        }

        [TestMethod]
        public void SkipNotSameDayFinishJourneysGetSetWorks()
        {
            p.SkipNotSameDayFinishJourneys = true;
            Assert.IsTrue(p.SkipNotSameDayFinishJourneys);
        }

        [TestMethod]
        public void OnlyPrintPathsGetSetWorks()
        {
            p.OnlyPrintPaths = true;
            Assert.IsTrue(p.OnlyPrintPaths);
        }

        [TestMethod]
        public void FileSavePathGetSetWorks()
        {
            p.FileSavePath = "asd";
            Assert.IsTrue(p.FileSavePath.Equals("asd"));
        }
        
        [TestMethod]
        public void HomeGetSetWorks()
        {
            p.Home = "asd";
            Assert.IsTrue(p.Home.Equals("asd"));
        }

        [TestMethod]
        public void EuropeOnlyGetSetWorks()
        {
            p.EuropeOnly = true;
            Assert.IsTrue(p.EuropeOnly);
        }

        [TestMethod]
        public void UKAndBulgariaOnlyGetSetWorks()
        {
            p.UKAndBulgariaOnly = true;
            Assert.IsTrue(p.UKAndBulgariaOnly);
        }

        [TestMethod]
        public void DefaultDelayGetSetWorks()
        {
            p.DefaultDelay = 500;
            Assert.IsTrue(p.DefaultDelay == 500);
        }

        [TestMethod]
        public void NoLongerThanGetSetWorks()
        {
            p.NoLongerThan = 500;
            Assert.IsTrue(p.NoLongerThan == 500);
        }
        
        [TestMethod]
        public void HotelCostGetSetWorks()
        {
            p.HotelCost = 500;
            Assert.IsTrue(p.HotelCost == 500);
        }

        [TestMethod]
        public void EarlyFlightHourGetSetWorks()
        {
            p.EarlyFlightHour = 500;
            Assert.IsTrue(p.EarlyFlightHour == 500);
        }
        
        [TestMethod]
        public void OnlyIncludeShortestPathsGetSetWorks()
        {
            p.OnlyIncludeShortestPaths = true;
            Assert.IsTrue(p.OnlyIncludeShortestPaths);
        }

        [TestMethod]
        public void HeadlessGetSetWorks()
        {
            p.Headless = true;
            Assert.IsTrue(p.Headless);
        }
    }
}
