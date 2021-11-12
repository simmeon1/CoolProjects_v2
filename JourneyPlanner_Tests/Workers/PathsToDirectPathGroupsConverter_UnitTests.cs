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
        private Path path1 = new(new List<string>() { "ABZ", "LTN", "SOF" });
        private Path path2 = new(new List<string>() { "LTN", "SOF", "EDI" });

        [TestMethod]
        public void DataGroupedAsExpected()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();
            Dictionary<string, string> translations = new();
            translations.Add("ABZ", "Aberdeen");
            JourneyRetrieverData data = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, translations);
            existingData.Add(megabus, data);

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1, path2 }, existingData);

            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results.ContainsKey(megabus));
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results[megabus].DirectPaths.Count == 1);
            Assert.IsTrue(results[megabus].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[megabus].Translations.Equals(translations));
            Assert.IsTrue(results[defaultWorker].DirectPaths.Count == 2);
            Assert.IsTrue(results[defaultWorker].DirectPaths[0].ToString().Equals("LTN-SOF"));
            Assert.IsTrue(results[defaultWorker].DirectPaths[1].ToString().Equals("SOF-EDI"));
            Assert.IsTrue(results[defaultWorker].Translations.Count == 0);
        }

        [TestMethod]
        public void DataGroupedAsExpected_NoGoogleFlights()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();

            Dictionary<string, string> translations1 = new();
            translations1.Add("ABZ", "Aberdeen");
            JourneyRetrieverData data1 = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, translations1);
            existingData.Add(megabus, data1);
            
            Dictionary<string, string> translations2 = null;
            JourneyRetrieverData data2 = new(new List<DirectPath>() { new DirectPath("LTN", "SOF"), new DirectPath("SOF", "EDI") }, translations2);
            existingData.Add(scotrail, data2);

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1, path2 }, existingData);

            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results.ContainsKey(megabus));
            Assert.IsTrue(results.ContainsKey(scotrail));
            Assert.IsTrue(results[megabus].DirectPaths.Count == 1);
            Assert.IsTrue(results[megabus].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[megabus].Translations.Equals(translations1));
            Assert.IsTrue(results[scotrail].DirectPaths.Count == 2);
            Assert.IsTrue(results[scotrail].DirectPaths[0].ToString().Equals("LTN-SOF"));
            Assert.IsTrue(results[scotrail].DirectPaths[1].ToString().Equals("SOF-EDI"));
            Assert.IsTrue(results[scotrail].Translations.Count == 0);
        }

        [TestMethod]
        public void DataGroupedAsExpected_SamePathsForDifferentCompanies()
        {
            Dictionary<string, JourneyRetrieverData> existingData = new();
            JourneyRetrieverData data1 = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, null);
            existingData.Add(megabus, data1);
            existingData.Add(scotrail, data1);

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(new List<Path>() { path1 }, existingData);

            Assert.IsTrue(results.Count == 3);
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results.ContainsKey(megabus));
            Assert.IsTrue(results.ContainsKey(scotrail));
            Assert.IsTrue(results[megabus].DirectPaths.Count == 1);
            Assert.IsTrue(results[megabus].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[scotrail].DirectPaths.Count == 1);
            Assert.IsTrue(results[scotrail].DirectPaths[0].ToString().Equals("ABZ-LTN"));
        }


        [TestMethod]
        public void DataGroupedAsExpected_NoExistingData()
        {
            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results =  converter.GetGroups(new List<Path>() { path1, path2 });

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results.ContainsKey(defaultWorker));
            Assert.IsTrue(results[defaultWorker].DirectPaths.Count == 3);
            Assert.IsTrue(results[defaultWorker].DirectPaths[0].ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(results[defaultWorker].DirectPaths[1].ToString().Equals("LTN-SOF"));
            Assert.IsTrue(results[defaultWorker].DirectPaths[2].ToString().Equals("SOF-EDI"));
            Assert.IsTrue(results[defaultWorker].Translations.Count == 0);
        }
    }
}
