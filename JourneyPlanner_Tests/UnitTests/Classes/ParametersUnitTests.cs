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
        public void FileSavePathGetSetWorks()
        {
            p.FileSavePath = "asd";
            Assert.IsTrue(p.FileSavePath.Equals("asd"));
        }
        
        [TestMethod]
        public void ExistingResultsPathGetSetWorks()
        {
            p.ExistingResultsPath = "asd";
            Assert.IsTrue(p.ExistingResultsPath.Equals("asd"));
        }
        
        [TestMethod]
        public void PenaltiesFileGetSetWorks()
        {
            p.TimePenaltiesFile = "asd";
            Assert.IsTrue(p.TimePenaltiesFile.Equals("asd"));
        }
        
        [TestMethod]
        public void NoLongerThanGetSetWorks()
        {
            p.NoLongerThan = 500;
            Assert.IsTrue(p.NoLongerThan == 500);
        }
        
        [TestMethod]
        public void HeadlessGetSetWorks()
        {
            p.Headless = true;
            Assert.IsTrue(p.Headless);
        }
    }
}
