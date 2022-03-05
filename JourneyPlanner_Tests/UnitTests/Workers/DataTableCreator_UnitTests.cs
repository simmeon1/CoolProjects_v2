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
            airportList = new List<Airport>() { airport1, airport2, airport3 };

            flight1 = new Journey(flight1Departing, flight1Arriving, Flight1Airline, flight1Span, Flight1Path, 25);
            flight2Departing = new DateTime(2000, 11, 11, 14, 0, 0);
            flight2Arriving = new DateTime(2000, 11, 11, 18, 0, 0);
            flight2Span = new TimeSpan(2, 0, 0);
            flight2 = new Journey(flight2Departing, flight2Arriving, Flight2Airline, flight2Span, Flight2Path, 50);

            creator = new DataTableCreator();
            correctSeq = new SequentialJourneyCollection(new JourneyCollection(new List<Journey>() { flight1, flight2 }));
            flights = new List<SequentialJourneyCollection>() { correctSeq };
        }

        [TestMethod]
        public void CorrectTables_AllColumns()
        {
            List<DataTable> tables = creator.GetTables(airportList, flights, false, false, false);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            int index = 0;
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 13);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(2));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(0));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].ToString().Equals(DataTableCreator.GetShortDateTime(flight1Departing)));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].ToString().Equals(DataTableCreator.GetShortDateTime(flight2Arriving)));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals("06:00"));
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[index++] == 1);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[index++] == 75.0);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[index++] == 0);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(false));

            int subIndex = 0;
            Assert.IsTrue(subTable.TableName.Equals("Details"));
            Assert.IsTrue(subTable.Rows.Count == 2);
            Assert.IsTrue(subTable.Rows[0].ItemArray.Length == 14);
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(Flight1Path));
            Assert.IsTrue((int)subTable.Rows[0].ItemArray[subIndex++] == 1);
            Assert.IsTrue((int)subTable.Rows[0].ItemArray[subIndex++] == 1);
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(true));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortDateTime(flight1Departing)));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortDateTime(flight1Arriving)));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(airport1.City));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(airport2.City));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(airport1.Country));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(airport2.Country));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortTimeSpan(flight1Span)));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortTimeSpan(new TimeSpan())));
            Assert.IsTrue(subTable.Rows[0].ItemArray[subIndex++].Equals(Flight1Airline));
            Assert.IsTrue((double)subTable.Rows[0].ItemArray[subIndex++] == 25);

            subIndex = 0;
            Assert.IsTrue(subTable.Rows[1].ItemArray.Length == 14);
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(Flight2Path));
            Assert.IsTrue((int)subTable.Rows[1].ItemArray[subIndex++] == 1);
            Assert.IsTrue((int)subTable.Rows[1].ItemArray[subIndex++] == 2);
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(true));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortDateTime(flight2Departing)));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortDateTime(flight2Arriving)));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(airport2.City));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(airport3.City));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(airport2.Country));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(airport3.Country));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortTimeSpan(flight2Span)));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].ToString().Equals(DataTableCreator.GetShortTimeSpan(new TimeSpan(2, 29, 20))));
            Assert.IsTrue(subTable.Rows[1].ItemArray[subIndex++].Equals(Flight2Airline));
            Assert.IsTrue((double)subTable.Rows[1].ItemArray[subIndex++] == 50);
        }

        [TestMethod]
        public void CorrectTables_SomeColumns()
        {
            List<DataTable> tables = creator.GetTables(airportList, flights, true, true, false);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            int index = 0;
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 11);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(2));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(0));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].ToString().Equals(DataTableCreator.GetShortDateTime(flight1Departing)));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].ToString().Equals(DataTableCreator.GetShortDateTime(flight2Arriving)));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals("06:00"));
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[index++] == 1);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[index++] == 75);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[index++] == 0);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[index++].Equals(false));
        }

        [TestMethod]
        public void CorrectTables_CountryChangesIsCorrect()
        {
            flight1Departing = new DateTime(2000, 11, 11, 10, 0, 00);
            flight1Arriving = new DateTime(2000, 11, 11, 11, 30, 40);
            flight1Span = new TimeSpan(1, 10, 40);
            const string flight1Airline = "easyJet";
            const string flight1Path = "ABZ-VAR";

            airport1 = new Airport("ABZ", "", "UK", "", "");
            airport2 = new Airport("VAR", "", "BG", "", "");
            airportList = new List<Airport> { airport1, airport2 };

            flight1 = new Journey(flight1Departing, flight1Arriving, flight1Airline, flight1Span, flight1Path, 25);
            creator = new DataTableCreator();
            correctSeq = new SequentialJourneyCollection(new JourneyCollection(new List<Journey>() { flight1 }));
            flights = new List<SequentialJourneyCollection> { correctSeq };
            List<DataTable> tables = creator.GetTables(airportList, flights, true, true, false);
            DataTable mainTable = tables[0];
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[7] == 1);
        }
    }
}
