using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class DataTableCreator_UnitTests
    {
        const string flight1Airline = "easyJet";
        const string flight1Path = "ABZ-EDI";
        const string flight2Airline = "wizz";
        const string flight2Path = "EDI-VAR";
        private DateTime flight1Departing;
        private DateTime flight1Arriving;
        private TimeSpan flight1span;
        private Airport airport1;
        private Airport airport2;
        private Airport airport3;
        private List<Airport> airportList;
        private Flight flight1;
        private DateTime flight2Departing;
        private DateTime flight2Arriving;
        private TimeSpan flight2span;
        private Flight flight2;
        private DataTableCreator creator;
        private SequentialFlightCollection correctSeq;
        private List<SequentialFlightCollection> flights;

        [TestInitialize]
        public void TestInitialize()
        {
            flight1Departing = new DateTime(2000, 11, 11, 10, 0, 00);
            flight1Arriving = new DateTime(2000, 11, 11, 11, 30, 40);
            flight1span = new TimeSpan(1, 10, 40);

            airport1 = new Airport("ABZ", "", "UK", "", "");
            airport2 = new Airport("EDI", "", "UK", "", "");
            airport3 = new Airport("VAR", "", "BG", "", "");
            airportList = new List<Airport>() { airport1, airport2, airport3 };

            flight1 = new Flight(flight1Departing, flight1Arriving, flight1Airline, flight1span, flight1Path, 25);
            flight2Departing = new DateTime(2000, 11, 11, 14, 0, 0);
            flight2Arriving = new DateTime(2000, 11, 11, 18, 0, 0);
            flight2span = new TimeSpan(2, 0, 0);
            flight2 = new Flight(flight2Departing, flight2Arriving, flight2Airline, flight2span, flight2Path, 50);

            creator = new DataTableCreator();
            correctSeq = new SequentialFlightCollection(new FlightCollection(new List<Flight>() { flight1, flight2 }));
            flights = new List<SequentialFlightCollection>() { correctSeq };
        }

        [TestMethod]
        public void CorrectTables_AllColumns()
        {
            List<DataTable> tables = creator.GetTables(airportList, flights, false, false, 100);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 11);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[0].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[1].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[2].Equals(2));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[3].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[4].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[5].Equals(flight1Departing.ToString()));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[6].Equals(flight2Arriving.ToString()));
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[7] == 8);
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[8] == 1);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[9] == 75);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[10] == 0);

            Assert.IsTrue(subTable.TableName.Equals("Details"));
            Assert.IsTrue(subTable.Rows.Count == 2);
            Assert.IsTrue(subTable.Rows[0].ItemArray.Length == 8);
            Assert.IsTrue(subTable.Rows[0].ItemArray[0].Equals(flight1Path));
            Assert.IsTrue((int)subTable.Rows[0].ItemArray[1] == 1);
            Assert.IsTrue((int)subTable.Rows[0].ItemArray[2] == 1);
            Assert.IsTrue(subTable.Rows[0].ItemArray[3].Equals(flight1Departing.ToString()));
            Assert.IsTrue(subTable.Rows[0].ItemArray[4].Equals(flight1Arriving.ToString()));
            Assert.IsTrue(subTable.Rows[0].ItemArray[5].Equals(flight1span.ToString()));
            Assert.IsTrue(subTable.Rows[0].ItemArray[6].Equals(flight1Airline));
            Assert.IsTrue((double)subTable.Rows[0].ItemArray[7] == 25);

            Assert.IsTrue(subTable.Rows[1].ItemArray.Length == 8);
            Assert.IsTrue(subTable.Rows[1].ItemArray[0].Equals(flight2Path));
            Assert.IsTrue((int)subTable.Rows[1].ItemArray[1] == 1);
            Assert.IsTrue((int)subTable.Rows[1].ItemArray[2] == 2);
            Assert.IsTrue(subTable.Rows[1].ItemArray[3].Equals(flight2Departing.ToString()));
            Assert.IsTrue(subTable.Rows[1].ItemArray[4].Equals(flight2Arriving.ToString()));
            Assert.IsTrue(subTable.Rows[1].ItemArray[5].Equals(flight2span.ToString()));
            Assert.IsTrue(subTable.Rows[1].ItemArray[6].Equals(flight2Airline));
            Assert.IsTrue((double)subTable.Rows[1].ItemArray[7] == 50);
        }

        [TestMethod]
        public void CorrectTables_SomeColumns()
        {
            List<DataTable> tables = creator.GetTables(airportList, flights, true, true, 100);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 9);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[0].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[1].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[2].Equals(2));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[3].Equals(flight1Departing.ToString()));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[4].Equals(flight2Arriving.ToString()));
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[5] == 8);
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[6] == 1);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[7] == 75);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[8] == 0);
        }

        [TestMethod]
        public void CorrectTables_FlightTooLong()
        {
            Assert.IsTrue(creator.GetTables(airportList, flights, true, true, 7)[0].Rows.Count == 0);
            Assert.IsTrue(creator.GetTables(airportList, flights, true, true, 8)[0].Rows.Count == 1);
            Assert.IsTrue(creator.GetTables(airportList, flights, true, true, 9)[0].Rows.Count == 1);
        }

        [TestMethod]
        public void CorrectTables_CountryChangesIsCorrect()
        {
            DateTime flight1Departing = new(2000, 11, 11, 10, 0, 00);
            DateTime flight1Arriving = new(2000, 11, 11, 11, 30, 40);
            TimeSpan flight1span = new(1, 10, 40);
            const string flight1Airline = "easyJet";
            const string flight1Path = "ABZ-VAR";

            Airport airport1 = new("ABZ", "", "UK", "", "");
            Airport airport2 = new("VAR", "", "BG", "", "");
            List<Airport> airportList = new() { airport1, airport2 };

            Flight flight1 = new(flight1Departing, flight1Arriving, flight1Airline, flight1span, flight1Path, 25);
            DataTableCreator creator = new();
            SequentialFlightCollection correctSeq = new(new FlightCollection(new List<Flight>() { flight1 }));
            List<SequentialFlightCollection> flights = new() { correctSeq };
            List<DataTable> tables = creator.GetTables(airportList, flights, true, true, 100);
            DataTable mainTable = tables[0];
            Assert.IsTrue((int)mainTable.Rows[0].ItemArray[6] == 1);
        }
    }
}
