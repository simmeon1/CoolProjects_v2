using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class SequentialFlightCollection_UnitTests
    {
        private readonly Airport airport1 = new("ABZ", country: "UK");
        private readonly Airport airport2 = new("EDI", country: "UK");
        private readonly Airport airport3 = new("VAR", country: "BG");
        private readonly Airport airport4 = new("BOJ", country: "BG");
        private readonly Airport airport5 = new("LTN", country: "UK");

        private Flight flight1;
        private Flight flight2;
        private Flight flight3;
        private Flight flight4;
        private Flight flight4Copy;

        [TestInitialize]
        public void TestInitialize()
        {
            List<Airport> airportList = new() { airport1, airport2, airport3, airport4, airport5 };
            flight1 = new(new DateTime(2000, 11, 11, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(1, 10, 40), airport1, airport2, 25);
            flight2 = new(new DateTime(2000, 11, 11, 14, 0, 0), new DateTime(2000, 11, 11, 18, 0, 0), "wizz", new TimeSpan(2, 0, 0), airport2, airport3, 50);
            flight3 = new(new DateTime(2000, 11, 11, 21, 30, 0), new DateTime(2000, 11, 11, 23, 0, 0), "wizz", new TimeSpan(1, 30, 0), airport3, airport4, 10);
            flight4 = new(new DateTime(2000, 11, 11, 23, 30, 0), new DateTime(2000, 11, 12, 1, 0, 0), "wizz", new TimeSpan(3, 30, 00), airport4, airport5, 40);
            flight4Copy = new(new DateTime(2000, 11, 11, 23, 30, 0), new DateTime(2000, 11, 12, 1, 0, 0), "wizzd", new TimeSpan(3, 30, 00), airport4, airport5, 40);
        }

        [TestMethod]
        public void ExceptionIsThrownForNoSequence()
        {
            FlightCollection collection = new(new List<Flight>() { flight1, flight3 });
            Assert.ThrowsException<Exception>(() => new SequentialFlightCollection(collection));
        }

        [TestMethod]
        public void SerialiseDeserialize()
        {
            SequentialFlightCollection collection = CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4);
            string json = collection.SerializeObject();
            SequentialFlightCollection collection2 = json.DeserializeObject<SequentialFlightCollection>();
            Assert.IsTrue(collection.ToString().Equals(collection2.ToString()));
        }

        [TestMethod]
        public void ToStringIsCorrectWithSomeFlights()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4).ToString().Equals("ABZ-EDI-VAR-BOJ-LTN, Doable = False, Start = 11/11/2000 10:20:30, End = 12/11/2000 01:00:00, Cost = 125"));
        }

        [TestMethod]
        public void ToStringIsCorrectWithNullFlights()
        {
            Assert.IsTrue(new SequentialFlightCollection(null).ToString().Equals("No flights in sequence."));
        }

        [TestMethod]
        public void ToStringIsCorrectWithNoFlights()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights().ToString().Equals("No flights in sequence."));
        }

        [TestMethod]
        public void ToStringIsCorrectWithNoFlightCollection()
        {
            Assert.IsTrue(new SequentialFlightCollection().ToString().Equals("No flights in sequence."));
        }

        [TestMethod]
        public void GetSetWorks()
        {
            SequentialFlightCollection seqCollection = CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4);
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
        public void CountIsCorrectWhenNotValid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).Count() == 0);
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
        public void SequenceIsValidAndNotDoableWithMultipleFlights()
        {
            Assert.IsFalse(CreateSeqCollectionWithFlights(flight3, flight4).SequenceIsDoable());
        }

        [TestMethod]
        public void SequenceIsInvalidAndNotDoable()
        {
            Assert.IsFalse(CreateSeqCollectionWithFlights(null).SequenceIsDoable());
        }

        [TestMethod]
        public void CostIsZeroWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetCost() == 0);
        }

        [TestMethod]
        public void CostIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetCost() == 75);
        }

        [TestMethod]
        public void GetStartTimeIsNullWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetStartTime() == null);
        }

        [TestMethod]
        public void GetStartTimeIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetStartTime() == flight1.Departing);
        }

        [TestMethod]
        public void GetEndTimeIsNullWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetEndTime() == null);
        }

        [TestMethod]
        public void GetEndTimeIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetEndTime() == flight2.Arriving);
        }

        [TestMethod]
        public void GetTotalTimeIsZeroWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetTotalTime() == 0);
        }

        [TestMethod]
        public void GetTotalTimeIsCorrect()
        {
            double time = CreateSeqCollectionWithFlights(flight1, flight2).GetTotalTime();
            Assert.IsTrue(time > 7 && time < 8);
        }

        [TestMethod]
        public void GetTotalTimeInFlightsIsZeroWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetTotalTimeInFlights() == 0);
        }

        [TestMethod]
        public void GetTotalTimeInFlightsIsCorrect()
        {
            double time = CreateSeqCollectionWithFlights(flight1, flight2).GetTotalTimeInFlights();
            Assert.IsTrue(time > 3 && time < 4);
        }

        [TestMethod]
        public void GetTotalIdleTimeIsZeroWhenInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetTotalIdleTime() == 0);
        }

        [TestMethod]
        public void GetTotalIdleTimeIsCorrect()
        {
            double time = CreateSeqCollectionWithFlights(flight1, flight2).GetTotalIdleTime();
            Assert.IsTrue(time > 4 && time < 5);
        }

        [TestMethod]
        public void StartsAndEndsOnSameDayIsFalseIfInvalid()
        {
            Assert.IsFalse(CreateSeqCollectionWithFlights(null).StartsAndEndsOnSameDay());
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
        public void CountryChangesIsCorrect()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1).GetCountryChanges() == 0);
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight2).GetCountryChanges() == 1);
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2).GetCountryChanges() == 1);
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2, flight3).GetCountryChanges() == 1);
            Assert.IsTrue(CreateSeqCollectionWithFlights(flight1, flight2, flight3, flight4).GetCountryChanges() == 2);
        }
        
        [TestMethod]
        public void CountryChangesIsZeroIfInvalid()
        {
            Assert.IsTrue(CreateSeqCollectionWithFlights(null).GetCountryChanges() == 0);
        }

        private static SequentialFlightCollection CreateSeqCollectionWithFlights(params Flight[] flights)
        {
            return new(flights == null ? null : new FlightCollection(flights.ToList()));
        }
    }
}
