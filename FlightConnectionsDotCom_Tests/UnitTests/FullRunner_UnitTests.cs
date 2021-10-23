using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
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
            parameters.OpenGoogleFlights = true;
            parameters.EuropeOnly = true;

            Mock<IFlightConnectionsDotComWorker_AirportCollector> collectorMock = new();
            List<Airport> airportList = new() { new Airport("LTN", "London", "United Kingdom", "Luton", ""), new Airport("VAR", "Varna", "Bulgaria", "Varna", "") };
            collectorMock.Setup(x => x.CollectAirports(It.IsAny<int>())).Returns(airportList);

            Mock<IFlightConnectionsDotComWorker_AirportPopulator> populatorMock = new();
            Dictionary<string, HashSet<string>> destinations = new();
            destinations.Add("LTN", new HashSet<string>() { "VAR" });
            populatorMock.Setup(x => x.PopulateAirports(It.IsAny<List<Airport>>(), It.IsAny<IAirportFilterer>())).Returns(destinations);

            Mock<IChromeWorker> chromeWorkerMock = new();
            List<PathAndFlightCollection> data = new();
            data.Add(new PathAndFlightCollection(new Path(new List<string>() { "VAR", "LTN" }), new FlightCollection()));
            FullPathAndListOfPathsAndFlightCollections fullPathAndFlights = new(new Path(new List<string>() { "VAR", "LTN" }), data);

            List<FullPathAndListOfPathsAndFlightCollections> fullPaths = new() { fullPathAndFlights };
            Dictionary<string, FlightCollection> workerFlights = new();
            workerFlights.Add("ABZ-LTN", new());
            ChromeWorkerResults workerResults = new(workerFlights, fullPaths);
            chromeWorkerMock.Setup(x => x.ProcessPaths(It.IsAny<List<Path>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<Dictionary<string, FlightCollection>>()).Result)
                    .Returns(workerResults);

            Mock<IFileIO> fileIOMock = new();
            Mock<IExcelPrinter> printerMock = new();
            FullRunner runner = new(
                new Mock<ILogger>().Object,
                new Mock<IDelayer>().Object,
                fileIOMock.Object,
                new Mock<IDateTimeProvider>().Object
,
                printerMock.Object,
                new Mock<IWebDriver>().Object,
                new Mock<IWebDriverWait>().Object,
                collectorMock.Object,
                populatorMock.Object,
                chromeWorkerMock.Object);
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
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_pathsAndFlights.json",
                workerResults.SerializeObject(Formatting.Indented)
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
            parameters.OpenGoogleFlights = true;
            parameters.UKAndBulgariaOnly = true;
            parameters.LocalAirportListFile = "LocalAirportListFile";
            parameters.LocalAirportDestinationsFile = "LocalAirportDestinationsFile";
            parameters.LocalChromeWorkerResultsFile = "LocalChromeWorkerResultsFile";
            Mock<IFileIO> fileIOMock = new();
            List<Airport> airportList = new() { new Airport("LTN", "London", "United Kingdom", "Luton", ""), new Airport("VAR", "Varna", "Bulgaria", "Varna", "") };
            fileIOMock.Setup(x => x.ReadAllText(parameters.LocalAirportListFile)).Returns(airportList.SerializeObject());

            Dictionary<string, HashSet<string>> destinations = new();
            destinations.Add("LTN", new HashSet<string>() { "VAR" });
            fileIOMock.Setup(x => x.ReadAllText(parameters.LocalAirportDestinationsFile)).Returns(destinations.SerializeObject());

            Mock<IChromeWorker> chromeWorkerMock = new();
            List<PathAndFlightCollection> data = new();
            data.Add(new PathAndFlightCollection(new Path(new List<string>() { "VAR", "LTN" }), new FlightCollection()));
            FullPathAndListOfPathsAndFlightCollections fullPathAndFlights = new(new Path(new List<string>() { "VAR", "LTN" }), data);

            List<FullPathAndListOfPathsAndFlightCollections> fullPaths = new() { fullPathAndFlights };
            Dictionary<string, FlightCollection> workerFlights = new();
            workerFlights.Add("ABZ-LTN", new());
            ChromeWorkerResults workerResults = new(workerFlights, fullPaths);
            fileIOMock.Setup(x => x.ReadAllText(parameters.LocalChromeWorkerResultsFile)).Returns(workerResults.SerializeObject());

            Mock<IExcelPrinter> printerMock = new();
            Mock<IFlightConnectionsDotComWorker_AirportCollector> collectorMock = new();
            Mock<IFlightConnectionsDotComWorker_AirportPopulator> populatorMock = new();
            FullRunner runner = new(
                new Mock<ILogger>().Object,
                new Mock<IDelayer>().Object,
                fileIOMock.Object,
                new Mock<IDateTimeProvider>().Object
,
                printerMock.Object,
                new Mock<IWebDriver>().Object,
                new Mock<IWebDriverWait>().Object,
                collectorMock.Object,
                populatorMock.Object,
                chromeWorkerMock.Object);
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
