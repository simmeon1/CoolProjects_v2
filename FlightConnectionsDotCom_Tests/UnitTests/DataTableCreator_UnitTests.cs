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
        [TestMethod]
        public void CorrectTables_AllColumns()
        {
            DateTime flight1Departing = new(2000, 11, 11, 10, 0, 00);
            DateTime flight1Arriving = new(2000, 11, 11, 11, 30, 40);
            TimeSpan flight1span = new(1, 10, 40);
            const string flight1Airline = "easyJet";
            const string flight1Path = "ABZ-EDI";
            Flight flight1 = new(flight1Departing, flight1Arriving, flight1Airline, flight1span, flight1Path, 25);
            DateTime flight2Departing = new(2000, 11, 11, 14, 0, 0);
            DateTime flight2Arriving = new(2000, 11, 11, 18, 0, 0);
            TimeSpan flight2span = new(2, 0, 0);
            const string flight2Airline = "wizz";
            const string flight2Path = "EDI-VAR";
            Flight flight2 = new(flight2Departing, flight2Arriving, flight2Airline, flight2span, flight2Path, 50);

            DataTableCreator creator = new();
            SequentialFlightCollection correctSeq = new(new FlightCollection(new List<Flight>() { flight1, flight2 }));
            List<SequentialFlightCollection> flights = new() { correctSeq };
            List<DataTable> tables = creator.GetTables(flights, false, false);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 8);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[0].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[1].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[2].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[3].Equals(true));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[4].Equals(flight1Departing.ToString()));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[5].Equals(flight2Arriving.ToString()));
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[6] == 8);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[7] == 75);
            
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
            DateTime flight1Departing = new(2000, 11, 11, 10, 0, 00);
            DateTime flight1Arriving = new(2000, 11, 11, 11, 30, 40);
            TimeSpan flight1span = new(1, 10, 40);
            const string flight1Airline = "easyJet";
            const string flight1Path = "ABZ-EDI";
            Flight flight1 = new(flight1Departing, flight1Arriving, flight1Airline, flight1span, flight1Path, 25);
            DateTime flight2Departing = new(2000, 11, 11, 14, 0, 0);
            DateTime flight2Arriving = new(2000, 11, 11, 18, 0, 0);
            TimeSpan flight2span = new(2, 0, 0);
            const string flight2Airline = "wizz";
            const string flight2Path = "EDI-VAR";
            Flight flight2 = new(flight2Departing, flight2Arriving, flight2Airline, flight2span, flight2Path, 50);

            DataTableCreator creator = new();
            SequentialFlightCollection correctSeq = new(new FlightCollection(new List<Flight>() { flight1, flight2 }));
            List<SequentialFlightCollection> flights = new() { correctSeq };
            List<DataTable> tables = creator.GetTables(flights, true, true);
            DataTable mainTable = tables[0];
            DataTable subTable = tables[1];
            Assert.IsTrue(mainTable.TableName.Equals("Summary"));
            Assert.IsTrue(mainTable.Rows.Count == 1);
            Assert.IsTrue(mainTable.Rows[0].ItemArray.Length == 6);
            Assert.IsTrue(mainTable.Rows[0].ItemArray[0].Equals("ABZ-EDI-VAR"));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[1].Equals(1));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[2].Equals(flight1Departing.ToString()));
            Assert.IsTrue(mainTable.Rows[0].ItemArray[3].Equals(flight2Arriving.ToString()));
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[4] == 8);
            Assert.IsTrue((double)mainTable.Rows[0].ItemArray[5] == 75);
        }
    }
}
