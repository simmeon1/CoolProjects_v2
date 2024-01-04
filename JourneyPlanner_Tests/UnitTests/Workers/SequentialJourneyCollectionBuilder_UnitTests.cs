using System;
using System.Collections.Generic;
using JourneyPlanner_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Workers
{
    [TestClass]
    public class SequentialJourneyCollectionBuilderUnitTests
    {        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_MultipleFlights()
        {
            Path path1 = new(new List<string> { "VAR", "LTN" });
            Path path2 = new(new List<string> { "LTN", "ABZ" });
            Path path3 = new(new List<string> { "ABZ", "EDI" });
            Path allPath = new(new List<string> { "VAR", "LTN", "ABZ", "EDI" });

            Journey flight1 = new(new DateTime(2000, 1, 13, 6, 5, 0), new DateTime(2000, 1, 13, 7, 45, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 52);
            Journey flight2 = new(new DateTime(2000, 1, 13, 19, 25, 0), new DateTime(2000, 1, 13, 21, 5, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 60);

            Journey flight3 = new(new DateTime(2000, 1, 13, 19, 0, 0), new DateTime(2000, 1, 13, 20, 25, 0), "easyJet", new TimeSpan(1, 25, 0), path2.ToString(), 23);
            Journey flight4 = new(new DateTime(2000, 1, 13, 7, 0, 0), new DateTime(2000, 1, 13, 10, 10, 0), "easyJet", new TimeSpan(3, 0, 0), path2.ToString(), 15);

            Journey flight5 = new(new DateTime(2000, 1, 13, 22, 0, 0), new DateTime(2000, 1, 13, 23, 0, 0), "easyJet", new TimeSpan(2, 0, 0), path3.ToString(), 15);
            Journey flight6 = new(new DateTime(2000, 1, 13, 11, 15, 0), new DateTime(2000, 1, 13, 13, 30, 0), "easyJet", new TimeSpan(4, 0, 0), path3.ToString(), 12);
            Journey flight7 = new(new DateTime(2000, 1, 13, 19, 15, 0), new DateTime(2000, 1, 13, 18, 30, 0), "easyJet", new TimeSpan(5, 0, 0), path3.ToString(), 10);

            SequentialJourneyCollectionBuilder collector = new();
            List<SequentialJourneyCollection> sequentialJourneys = collector.GetFullPathCombinationOfJourneys(
                new List<Path> { allPath }, new(new List<Journey> { flight1, flight2, flight3, flight4, flight5, flight6, flight7 }), 24);
            Assert.IsTrue(sequentialJourneys.Count == 1);
            Assert.IsTrue(sequentialJourneys[0][0].ToString().Equals(flight1.ToString()));
            Assert.IsTrue(sequentialJourneys[0][1].ToString().Equals(flight3.ToString()));
            Assert.IsTrue(sequentialJourneys[0][2].ToString().Equals(flight5.ToString()));
        }
        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_OneFlight()
        {
            Path path1 = new(new List<string>() { "VAR", "LTN" });
            Path shortPath = new(new List<string>() { "VAR", "LTN" });
            Path fullPath = new(new List<string>() { "VAR", "LTN", "ABZ" });
            Journey flight1 = new(new DateTime(2000, 1, 13, 6, 5, 0), new DateTime(2000, 1, 13, 7, 45, 0), "Wizz Air", new TimeSpan(3, 40, 0), path1.ToString(), 52);

            JourneyCollection journeys = new(new List<Journey>() { flight1 });
            SequentialJourneyCollectionBuilder collector = new();
            List<SequentialJourneyCollection> fullPathCombinationOfFlights = collector.GetFullPathCombinationOfJourneys(new List<Path>() { fullPath }, journeys, 24);
            Assert.IsTrue(fullPathCombinationOfFlights.Count == 0);
            
            fullPathCombinationOfFlights = collector.GetFullPathCombinationOfJourneys(new List<Path>() { shortPath, fullPath }, journeys, 24);
            Assert.IsTrue(fullPathCombinationOfFlights.Count == 1);
            Assert.IsTrue(fullPathCombinationOfFlights[0].Count() == 1);
            Assert.IsTrue(fullPathCombinationOfFlights[0][0].ToString().Equals(flight1.ToString()));
        }
        
        [TestMethod]
        public void GetFullPathCombinationOfFLights_NoFlights()
        {
            Path allPath = new(new List<string>() { "VAR", "LTN" });
            SequentialJourneyCollectionBuilder collector = new();
            Assert.IsTrue(collector.GetFullPathCombinationOfJourneys(new(), new(), 24).Count == 0);
            Assert.IsTrue(collector.GetFullPathCombinationOfJourneys(new List<Path>() { allPath }, new(), 24).Count == 0);
        }
    }
}
