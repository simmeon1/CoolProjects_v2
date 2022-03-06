using System;
using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class SequentialJourneyCollectionUnitTests
    {
        private readonly Journey flight1 = new(new DateTime(2000, 11, 11, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(1, 10, 10), "ABZ-EDI", 25);
        private readonly Journey bus1 = new(new DateTime(2000, 11, 11, 07, 00, 00), new DateTime(2000, 11, 11, 13, 00, 00), "easyJet", new TimeSpan(4, 0, 0), "ABZ-EDI", 15, "busWorker");
        private readonly Journey flight2 = new(new DateTime(2000, 11, 11, 14, 0, 0), new DateTime(2000, 11, 11, 18, 0, 0), "wizz", new TimeSpan(2, 0, 0), "EDI-VAR", 50);
        private readonly Journey flight3 = new(new DateTime(2000, 11, 11, 21, 30, 0), new DateTime(2000, 11, 11, 23, 0, 0), "wizz", new TimeSpan(1, 30, 0), "VAR-BOJ", 10);
        private readonly Journey bus3 = new(new DateTime(2000, 11, 11, 18, 40, 00), new DateTime(2000, 11, 11, 21, 00, 00), "easyJet", new TimeSpan(4, 0, 0), "VAR-BOJ", 15, "busWorker");
        private readonly Journey flight4 = new(new DateTime(2000, 11, 11, 23, 30, 0), new DateTime(2000, 11, 12, 1, 0, 0), "wizz", new TimeSpan(3, 30, 0), "BOJ-LTN", 40);
        private readonly Journey flight4Copy = new(new DateTime(2000, 11, 11, 23, 30, 0), new DateTime(2000, 11, 12, 1, 0, 0), "wizzd", new TimeSpan(3, 30, 0), "BOJ-LTN", 40);

        [TestMethod]
        public void ExceptionIsThrownForNoSequence()
        {
            JourneyCollection collection = new(new List<Journey>() { flight1, flight3 });
            Assert.ThrowsException<Exception>(() => new SequentialJourneyCollection(collection));
        }
        
        [TestMethod]
        public void ExceptionIsThrownForEmptySequence()
        {
            JourneyCollection collection = new(new List<Journey>());
            Assert.ThrowsException<Exception>(() => new SequentialJourneyCollection(collection));
        }

        [TestMethod]
        public void ToStringIsCorrectWithSomeFlights()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4).ToString().Equals("ABZ-EDI-VAR-BOJ-LTN, Doable = False, Start = 11/11/2000 10:20:30, End = 12/11/2000 01:00:00, Cost = 125"));
        }

        [TestMethod]
        public void GetSetWorks()
        {
            SequentialJourneyCollection seqCollection = CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4);
            seqCollection[3] = flight4Copy;
            Assert.IsTrue(seqCollection[3] == flight4Copy);
            seqCollection[3] = flight4;
            Assert.IsTrue(seqCollection[3] == flight4);
        }

        [TestMethod]
        public void GetSetThrowsException()
        {
            Assert.ThrowsException<Exception>(() => CreateSeqCollectionWithFlights(flight1, flight2, flight3)[0] = flight2);
        }

        [TestMethod]
        public void CountIsCorrectWhenValid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).Count() == 2);
        }

        [TestMethod]
        public void SequenceIsValidAndDoableWithOneFlight()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1).SequenceIsDoable());
        }

        [TestMethod]
        public void SequenceIsValidAndDoableWithMultipleFlights()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).SequenceIsDoable());
        }
        
        [TestMethod]
        public void SequenceIsValidAndDoableWithFlightAndBus()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight2, bus3).SequenceIsDoable());
        }

        [TestMethod]
        public void SequenceIsValidAndNotDoableWithMultipleFlights()
        {
            Assert.IsFalse(CreateSeqCollectionWithFlights(flight3, flight4).SequenceIsDoable());
        }

        [TestMethod]
        public void CostIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetCost() == 75);
        }

        [TestMethod]
        public void GetStartTimeIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetStartTime() == flight1.Departing);
        }

        [TestMethod]
        public void GetEndTimeIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetEndTime() == flight2.Arriving);
        }

        [TestMethod]
        public void GetTotalTimeIsCorrect()
        {
            TimeSpan time = CreateSeqCollectionWithFlights(flight1, flight2).GetLength();
            Assert.IsTrue(time.ToString().Equals("05:39:30"));
        }

        [TestMethod]
        public void StartsAndEndsOnSameDayIsTrue()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).StartsAndEndsOnSameDay());
        }

        [TestMethod]
        public void StartsAndEndsOnSameDayIsFalseIfNotTrue()
        {
            Assert.IsFalse(CreateSeqCollectionWithFlights(flight3, flight4).StartsAndEndsOnSameDay());
        }

        [TestMethod]
        public void CountOfFlightsIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight3, flight4).GetCountOfFlights() == 2);
        }

        [TestMethod]
        public void CountOfBusesIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(bus1, flight2).GetCountOfBuses() == 1);
        }

        [TestMethod]
        public void HasZeroCostIsCorrect()
        {
            Journey flight4CopyZeroCost = new(new DateTime(2000, 11, 11, 23, 30, 0), new DateTime(2000, 11, 12, 1, 0, 0), "wizzd", new TimeSpan(3, 30, 00), "BOJ-LTN", 0);
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight3, flight4CopyZeroCost).HasJourneyWithZeroCost());
        }
        
        [TestMethod]
        public void HasZeroCostIsIncorrect()
        {
            Assert.IsTrue(!CreateSeqCollectionWithFlights(flight3, flight4).HasJourneyWithZeroCost());
        }
        
        [TestMethod]
        public void GetDepartingLocationIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight3, flight4).GetDepartingLocation().Equals("VAR"));
        }
        
        [TestMethod]
        public void GetCountOfCompaniesIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight2, flight3).GetCountOfCompanies() == 1);
        }

        private static SequentialJourneyCollection CreateSeqCollectionWithFlights(params Journey[] flights)
        {
            return new SequentialJourneyCollection(flights == null ? null : new JourneyCollection(flights.ToList()));
        }
    }
}
