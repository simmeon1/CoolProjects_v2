using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class FullRunner_UnitTests
    {
        [TestMethod]
        public async Task RunIsSuccesful_NoFilesProvided()
        {
            Parameters parameters = new();
            parameters.Origins = new() { "LTN" };
            parameters.Destinations = new() { "VAR" };
            parameters.MaxFlights = 1;
            parameters.DateFrom = new(2020, 5, 20);
            parameters.DateTo = new(2021, 6, 21);
            parameters.FileSavePath = "C:\\D";
            parameters.OnlyPrintPaths = false;
            parameters.EuropeOnly = true;
            parameters.Headless = true;
            parameters.DefaultDelay = 500;

            Mock<IFlightConnectionsDotComWorker_AirportCollector> collectorMock = new();
            List<Airport> airportList = new() { new Airport("LTN", "London", "United Kingdom", "Luton", ""), new Airport("VAR", "Varna", "Bulgaria", "Varna", "") };
            collectorMock.Setup(x => x.CollectAirports(It.IsAny<int>())).Returns(airportList);

            Mock<IFlightConnectionsDotComWorker_AirportPopulator> populatorMock = new();
            Dictionary<string, HashSet<string>> destinations = new();
            destinations.Add("LTN", new HashSet<string>() { "VAR" });
            populatorMock.Setup(x => x.PopulateAirports(It.IsAny<List<Airport>>(), It.IsAny<IAirportFilterer>())).Returns(destinations);

            Mock<IMultiJourneyCollector> collector = new();
            collector.Setup(x =>
                x.GetJourneys(It.IsAny<JourneyRetrieverComponents>(), It.IsAny<Dictionary<string, JourneyRetrieverData>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<MultiJourneyCollectorResults>()).Result)
                .Returns(new MultiJourneyCollectorResults(new(), new()));
            JourneyRetrieverComponents c = new(
                new Mock<IJourneyRetrieverEventHandler>().Object,
                null,
                new Mock<ILogger>().Object,
                new Mock<IDelayer>().Object,
                500,
                null
            );

            Mock<IJourneyRetriever> retriever = new();
            retriever.Setup(x => x.CollectJourneys(It.IsAny<JourneyRetrieverData>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<JourneyCollection>()).Result).Returns(new JourneyCollection());

            Mock<IJourneyRetrieverInstanceCreator> creator = new();
            creator.Setup(x => x.CreateInstance(It.IsAny<string>(), c)).Returns(retriever.Object);

            Mock<IFileIO> fileIOMock = new();
            Mock<IExcelPrinter> printerMock = new();
            FullRunner runner = new(
                c,
                fileIOMock.Object,
                new Mock<IDateTimeProvider>().Object
,
                printerMock.Object,
                collectorMock.Object,
                populatorMock.Object,
                collector.Object
            );
            await runner.DoRun(parameters);
            const string directoryName = @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21";
            fileIOMock.Verify(x => x.DirectoryExists(directoryName), Times.Once());
            fileIOMock.Verify(x => x.CreateDirectory(directoryName), Times.Once());
            fileIOMock.Verify(x => x.WriteAllText(
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_airportList.json",
                airportList.SerializeObject(Formatting.Indented)
            ), Times.Once());
            fileIOMock.Verify(x => x.WriteAllText(
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_airportDestinations.json",
                destinations.SerializeObject(Formatting.Indented)
            ), Times.Once());
            fileIOMock.Verify(x => x.WriteAllText(
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_latestPaths.json",
                It.IsAny<string>()
            ), Times.Once());
            fileIOMock.Verify(x => x.WriteAllText(
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_journeyCollectorResults.json",
                new MultiJourneyCollectorResults(new(), new()).SerializeObject(Formatting.Indented)
            ), Times.Once());
            printerMock.Verify(x => x.PrintTablesToWorksheet(
                It.IsAny<List<DataTable>>(),
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_results.xlsx"
            ), Times.Once());
        }

        [TestMethod]
        public async Task RunIsSuccesful_FilesProvided()
        {
            Parameters parameters = new();
            parameters.Origins = new() { "LTN" };
            parameters.Destinations = new() { "VAR" };
            parameters.MaxFlights = 1;
            parameters.DateFrom = new(2020, 5, 20);
            parameters.DateTo = new(2021, 6, 21);
            parameters.FileSavePath = "C:\\D";
            parameters.OnlyPrintPaths = false;
            parameters.UKAndBulgariaOnly = true;
            parameters.AirportListFile = "LocalAirportListFile";
            parameters.AirportDestinationsFile = "LocalAirportDestinationsFile";
            parameters.ProgressFile = "LocalChromeWorkerResultsFile";
            parameters.WorkerSetupFile = "WorkerSetupFile";
            Mock<IFileIO> fileIOMock = new();
            List<Airport> airportList = new() { new Airport("LTN", "London", "United Kingdom", "Luton", ""), new Airport("VAR", "Varna", "Bulgaria", "Varna", "") };
            fileIOMock.Setup(x => x.ReadAllText(parameters.AirportListFile)).Returns(airportList.SerializeObject());

            Dictionary<string, HashSet<string>> destinations = new();
            destinations.Add("LTN", new HashSet<string>() { "VAR" });
            fileIOMock.Setup(x => x.ReadAllText(parameters.AirportDestinationsFile)).Returns(destinations.SerializeObject());

            fileIOMock.Setup(x => x.ReadAllText(parameters.WorkerSetupFile)).Returns(new Dictionary<string, JourneyRetrieverData>().SerializeObject());
            fileIOMock.Setup(x => x.ReadAllText(parameters.ProgressFile)).Returns(new MultiJourneyCollectorResults().SerializeObject());

            JourneyRetrieverComponents c = new(
                new Mock<IJourneyRetrieverEventHandler>().Object,
                null,
                new Mock<ILogger>().Object,
                new Mock<IDelayer>().Object,
                500,
                null
            );

            Mock<IExcelPrinter> printerMock = new();
            Mock<IFlightConnectionsDotComWorker_AirportCollector> airportCollectorMock = new();
            Mock<IFlightConnectionsDotComWorker_AirportPopulator> airportPopulatorMock = new();

            Mock<IMultiJourneyCollector> journeyCollectorMock = new();
            journeyCollectorMock.Setup(x =>
                x.GetJourneys(It.IsAny<JourneyRetrieverComponents>(), It.IsAny<Dictionary<string, JourneyRetrieverData>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<MultiJourneyCollectorResults>()).Result)
                .Returns(new MultiJourneyCollectorResults(new(), new()));

            FullRunner runner = new(
                c,
                fileIOMock.Object,
                new Mock<IDateTimeProvider>().Object
,
                printerMock.Object,
                airportCollectorMock.Object,
                airportPopulatorMock.Object,
                journeyCollectorMock.Object);
            await runner.DoRun(parameters);
            const string directoryName = @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21";
            fileIOMock.Verify(x => x.DirectoryExists(directoryName), Times.Once());
            fileIOMock.Verify(x => x.CreateDirectory(directoryName), Times.Once());
            fileIOMock.Verify(x => x.WriteAllText(
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_latestPaths.json",
                It.IsAny<string>()
            ), Times.Once());
            printerMock.Verify(x => x.PrintTablesToWorksheet(
                It.IsAny<List<DataTable>>(),
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_results.xlsx"
            ), Times.Once());
        }
    }
}
