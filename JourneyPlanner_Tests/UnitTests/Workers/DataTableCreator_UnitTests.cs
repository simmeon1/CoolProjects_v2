using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Workers
{
    [TestClass]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class DataTableCreatorUnitTests
    {
        private const string Flight1Airline = "easyJet";
        private const string Flight1Path = "ABZ-EDI";
        private const string Flight2Airline = "wizz";
        private const string Flight2Path = "EDI-VAR";
        private DateTime flight1Departing;
        private DateTime flight1Arriving;
        private TimeSpan flight1Span;
        private Airport airport1;
        private Airport airport2;
        private Airport airport3;
        private List<Airport> airportList;
        private Journey flight1;
        private DateTime flight2Departing;
        private DateTime flight2Arriving;
        private TimeSpan flight2Span;
        private Journey flight2;
        private DataTableCreator creator;
        private SequentialJourneyCollection correctSeq;
        private List<SequentialJourneyCollection> flights;

        [TestInitialize]
        public void TestInitialize()
        {
            flight1Departing = new DateTime(2000, 11, 11, 10, 0, 00);
            flight1Arriving = new DateTime(2000, 11, 11, 11, 30, 40);
            flight1Span = new TimeSpan(1, 30, 40);

            airport1 = new Airport("ABZ", "Aberdeen", "UK", "", "");
            airport2 = new Airport("EDI", "Edinburgh", "UK", "", "");
            airport3 = new Airport("VAR", "Varna", "BG", "", "");
            airportList = new List<Airport>() {airport1, airport2, airport3};

            flight1 = new Journey(flight1Departing, flight1Arriving, Flight1Airline, flight1Span, Flight1Path, 25);
            flight2Departing = new DateTime(2000, 11, 11, 14, 0, 0);
            flight2Arriving = new DateTime(2000, 11, 11, 18, 0, 0);
            flight2Span = new TimeSpan(2, 0, 0);
            flight2 = new Journey(flight2Departing, flight2Arriving, Flight2Airline, flight2Span, Flight2Path, 50);

            creator = new DataTableCreator();
            correctSeq = new SequentialJourneyCollection(new JourneyCollection(new List<Journey>() {flight1, flight2}));
            flights = new List<SequentialJourneyCollection>() {correctSeq};
        }

        [TestMethod]
        public void CorrectTables_AllColumns()
        {
            List<DataTable> tables = creator.GetTables(
                airportList,
                flights,
                false,
                false,
                "ABZ",
                20,
                50,
                100,
                11
            );
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            int index = 0;
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            object[] mainTableFirstRowItems = mainTable.Rows[0].ItemArray;
            Assert.IsTrue(mainTableFirstRowItems.Length == 18);
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(1));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(2));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(0));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(true));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(true));
            Assert.IsTrue(mainTableFirstRowItems[index++].ToString().Equals("11/11/2000 10:00:00"));
            Assert.IsTrue(mainTableFirstRowItems[index++].ToString().Equals("11/11/2000 18:00:00"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("06:00"));
            Assert.IsTrue((int) mainTableFirstRowItems[index++] == 2);
            Assert.IsTrue((int) mainTableFirstRowItems[index++] == 1);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 75.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 100.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 175.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 0);
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("easyJet, wizz"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(false));

            int subIndex = 0;
            Assert.IsTrue(subTable.TableName.Equals("Details"));
            Assert.IsTrue(subTable.Rows.Count == 2);
            object[] subTableFirstRowItems = subTable.Rows[0].ItemArray;
            Assert.IsTrue(subTableFirstRowItems.Length == 14);
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(Flight1Path));
            Assert.IsTrue((int) subTableFirstRowItems[subIndex++] == 1);
            Assert.IsTrue((int) subTableFirstRowItems[subIndex++] == 1);
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(true));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].ToString().Equals("11/11/2000 10:00:00"));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].ToString().Equals("11/11/2000 11:30:40"));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(airport1.City));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(airport2.City));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(airport1.Country));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(airport2.Country));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].Equals(Flight1Airline));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].ToString().Equals("00:00"));
            Assert.IsTrue(subTableFirstRowItems[subIndex++].ToString().Equals("01:30"));
            Assert.IsTrue((double) subTableFirstRowItems[subIndex++] == 25);

            subIndex = 0;
            object[] subTableSecondRowItems = subTable.Rows[1].ItemArray;
            Assert.IsTrue(subTableSecondRowItems.Length == 14);
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(Flight2Path));
            Assert.IsTrue((int) subTableSecondRowItems[subIndex++] == 1);
            Assert.IsTrue((int) subTableSecondRowItems[subIndex++] == 2);
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(true));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].ToString().Equals("11/11/2000 14:00:00"));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].ToString().Equals("11/11/2000 18:00:00"));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(airport2.City));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(airport3.City));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(airport2.Country));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(airport3.Country));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].Equals(Flight2Airline));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].ToString().Equals("02:29"));
            Assert.IsTrue(subTableSecondRowItems[subIndex++].ToString().Equals("02:00"));
            Assert.IsTrue((double) subTableSecondRowItems[subIndex++] == 50);
        }

        [TestMethod]
        public void CorrectTables_2()
        {
            List<DataTable> tables = creator.GetTables(
                airportList,
                flights,
                true,
                true,
                "FCO",
                20,
                50,
                100,
                11
            );
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            int index = 0;
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            object[] mainTableFirstRowItems = mainTable.Rows[0].ItemArray;
            Assert.IsTrue(mainTableFirstRowItems.Length == 16);
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(1));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(2));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(0));
            Assert.IsTrue(mainTableFirstRowItems[index++].ToString().Equals("11/11/2000 10:00:00"));
            Assert.IsTrue(mainTableFirstRowItems[index++].ToString().Equals("11/11/2000 18:00:00"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("06:00"));
            Assert.IsTrue((int) mainTableFirstRowItems[index++] == 2);
            Assert.IsTrue((int) mainTableFirstRowItems[index++] == 1);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 75.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 220.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 295.0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 0);
            Assert.IsTrue((double) mainTableFirstRowItems[index++] == 0);
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals("easyJet, wizz"));
            Assert.IsTrue(mainTableFirstRowItems[index++].Equals(false));
        }
    }
}