using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class SequentialJourneyCollectionBuilder_UnitTests
    {        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_MultipleFlights()
        {
            Path path1 = new(new List<string>() { "VAR", "LTN" });
            Path path2 = new(new List<string>() { "LTN", "ABZ" });
            Path path3 = new(new List<string>() { "ABZ", "EDI" });
            Path allPath = new(new List<string>() { "VAR", "LTN", "ABZ", "EDI" });

            Journey flight1 = new(new DateTime(2000, 1, 13, 6, 5, 0), new DateTime(2000, 1, 13, 7, 45, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 52);
            Journey flight2 = new(new DateTime(2000, 1, 13, 19, 25, 0), new DateTime(2000, 1, 13, 21, 5, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 60);

            Journey flight3 = new(new DateTime(2000, 1, 13, 19, 0, 0), new DateTime(2000, 1, 13, 20, 25, 0), "easyJet", new TimeSpan(1, 25, 0), path2.ToString(), 23);
            Journey flight4 = new(new DateTime(2000, 1, 13, 7, 0, 0), new DateTime(2000, 1, 13, 10, 10, 0), "easyJet", new TimeSpan(3, 0, 0), path2.ToString(), 15);

            Journey flight5 = new(new DateTime(2000, 1, 13, 22, 0, 0), new DateTime(2000, 1, 13, 23, 0, 0), "easyJet", new TimeSpan(2, 0, 0), path3.ToString(), 15);
            Journey flight6 = new(new DateTime(2000, 1, 13, 11, 15, 0), new DateTime(2000, 1, 13, 13, 30, 0), "easyJet", new TimeSpan(4, 0, 0), path3.ToString(), 12);
            Journey flight7 = new(new DateTime(2000, 1, 13, 19, 15, 0), new DateTime(2000, 1, 13, 18, 30, 0), "easyJet", new TimeSpan(5, 0, 0), path3.ToString(), 10);

            JourneyCollection journeys1 = new(new List<Journey>() { flight1, flight2 });
            JourneyCollection journeys2 = new(new List<Journey>() { flight3, flight4 });
            JourneyCollection journeys3 = new(new List<Journey>() { flight5, flight6, flight7 });
            List<JourneyCollection> journeysCollections = new() { journeys1, journeys2, journeys3 };

            SequentialJourneyCollectionBuilder collector = new();
            List<SequentialJourneyCollection> fullPathCombinationOfFlights = collector.GetFullPathCombinationOfJourneys(journeysCollections);
            Assert.IsTrue(fullPathCombinationOfFlights.Count == 12);
            Assert.IsTrue(fullPathCombinationOfFlights[0][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[1][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[2][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[3][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[4][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[5][0].ToString().Equals(flight1.ToString()));

            Assert.IsTrue(fullPathCombinationOfFlights[6][0].ToString().Equals(flight2.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[7][0].ToString().Equals(flight2.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[8][0].ToString().Equals(flight2.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[9][0].ToString().Equals(flight2.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[10][0].ToString().Equals(flight2.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[11][0].ToString().Equals(flight2.ToString()));

            Assert.IsTrue(fullPathCombinationOfFlights[0][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[1][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[2][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[6][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[7][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[8][1].ToString().Equals(flight3.ToString()));
            
            Assert.IsTrue(fullPathCombinationOfFlights[3][1].ToString().Equals(flight4.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[4][1].ToString().Equals(flight4.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[5][1].ToString().Equals(flight4.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[9][1].ToString().Equals(flight4.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[10][1].ToString().Equals(flight4.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[11][1].ToString().Equals(flight4.ToString()));
            
            Assert.IsTrue(fullPathCombinationOfFlights[0][2].ToString().Equals(flight5.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[3][2].ToString().Equals(flight5.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[6][2].ToString().Equals(flight5.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[9][2].ToString().Equals(flight5.ToString()));

            Assert.IsTrue(fullPathCombinationOfFlights[1][2].ToString().Equals(flight6.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[4][2].ToString().Equals(flight6.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[7][2].ToString().Equals(flight6.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[10][2].ToString().Equals(flight6.ToString()));
            
            Assert.IsTrue(fullPathCombinationOfFlights[2][2].ToString().Equals(flight7.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[5][2].ToString().Equals(flight7.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[8][2].ToString().Equals(flight7.ToString()));
            Assert.IsTrue(fullPathCombinationOfFlights[11][2].ToString().Equals(flight7.ToString()));
        }
        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_OneFlight()
        {
            Path path1 = new(new List<string>() { "VAR", "LTN" });
            Path allPath = new(new List<string>() { "VAR", "LTN", "ABZ" });
            Journey flight1 = new(new DateTime(2000, 1, 13, 6, 5, 0), new DateTime(2000, 1, 13, 7, 45, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 52);

            JourneyCollection journeys = new(new List<Journey>() { flight1 });
            SequentialJourneyCollectionBuilder collector = new();
            List<SequentialJourneyCollection> fullPathCombinationOfFlights = collector.GetFullPathCombinationOfJourneys(new List<JourneyCollection>() { journeys });
            Assert.IsTrue(fullPathCombinationOfFlights.Count == 1);
            Assert.IsTrue(fullPathCombinationOfFlights[0].Count() == 1);
            Assert.IsTrue(fullPathCombinationOfFlights[0][0].ToString().Equals(flight1.ToString()));
        }
        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_NoFlights()
        {
            Path allPath = new(new List<string>() { "VAR", "LTN" });
            SequentialJourneyCollectionBuilder collector = new();
            List<SequentialJourneyCollection> fullPathCombinationOfFlights = collector.GetFullPathCombinationOfJourneys(new List<JourneyCollection>());
            Assert.IsTrue(fullPathCombinationOfFlights.Count == 0);
        }
    }
}
