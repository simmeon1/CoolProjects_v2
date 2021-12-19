using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]

    public class PathsToDirectPathGroupsConverter_UnitTests
    {
        private const string megabus = "megabus";
        private const string scotrail = "scotrail";
        private const string defaultWorker = nameof(GoogleFlightsWorker);
        private readonly Path path1 = new(new List<string>() { "ABZ", "LTN", "SOF" });
        private readonly Path path2 = new(new List<string>() { "LTN", "SOF", "EDI" });
        private readonly Dictionary<string, HashSet<string>> airportsAndDestinations = new();


        [TestInitialize]
        public void TestInitialize()
        {
            airportsAndDestinations.Add("ABZ", new HashSet<string>() { "LTN" });
        }

        [TestMethod]
        public void DataGroupedAsExpected()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();
            Dictionary<string, string> translations = new();
            translations.Add("ABZ", "Aberdeen");
            JourneyRetrieverData data = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, translations);
            existingData.Add(megabus, data);

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1, path2 }, existingData, airportsAndDestinations);

            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results.ContainsKey(megabus));
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results[megabus].DirectPaths.Count == 1);
            Assert.IsTrue(results[megabus].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[megabus].Translations.Equals(translations));
            Assert.IsTrue(results[defaultWorker].DirectPaths.Count == 1);
            Assert.IsTrue(results[defaultWorker].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[defaultWorker].Translations.Count == 0);
        }

        [TestMethod]
        public void DataGroupedAsExpected_SamePathsForDifferentCompanies()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();
            JourneyRetrieverData data1 = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, null);
            existingData.Add(megabus, data1);
            existingData.Add(scotrail, data1);

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1 }, existingData, airportsAndDestinations);

            Assert.IsTrue(results.Count == 3);
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results.ContainsKey(megabus));
            Assert.IsTrue(results.ContainsKey(scotrail));
            Assert.IsTrue(results[megabus].DirectPaths.Count == 1);
            Assert.IsTrue(results[megabus].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[scotrail].DirectPaths.Count == 1);
            Assert.IsTrue(results[scotrail].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[defaultWorker].DirectPaths.Count == 1);
            Assert.IsTrue(results[defaultWorker].DirectPaths[0].ToString().Equals("ABZ-LTN"));
        }

        [TestMethod]
        public void DataGroupedAsExpected_NoExistingData()
        {
            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1, path2 }, new Dictionary<string, JourneyRetrieverData>(), airportsAndDestinations);

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results[defaultWorker].DirectPaths.Count == 1);
            Assert.IsTrue(results[defaultWorker].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[defaultWorker].Translations.Count == 0);
        }


        [TestMethod]
        public void DataGroupedAsExpected_NoGoogleFlights()
        {
            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1, path2 }, new Dictionary<string, JourneyRetrieverData>(), new Dictionary<string, HashSet<string>>());
            Assert.IsTrue(results.Count == 0);
        }
    }
}
