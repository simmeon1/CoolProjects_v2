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
        public async Task RunIsSuccesfulAsync()
        {
            Parameters parameters = new();
            parameters.Origins = new() { "LTN" };
            parameters.Destinations = new() { "VAR" };
            parameters.MaxFlights = 1;
            parameters.DateFrom = new(2020, 5, 20);
            parameters.DateTo = new(2021, 6, 21);
            parameters.FileSavePath = "C:\\D";
            parameters.OpenGoogleFlights = true;

            Mock<IFlightConnectionsDotComWorker_AirportCollector> collectorMock = new();
            List<Airport> airportList = new List<Airport>() { new Airport("LTN", "London", "UK", "Luton", ""), new Airport("VAR", "Varna", "Bulgaria", "Varna", "") };
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
            chromeWorkerMock.Setup(x => x.ProcessPaths(It.IsAny<List<Path>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()).Result).Returns(fullPaths);

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
                fullPaths.SerializeObject(Formatting.Indented)
            ), Times.Once());
            printerMock.Verify(x => x.PrintTablesToWorksheet(
                It.IsAny<List<DataTable>>(),
                @"C:\D\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21\0001-01-01--00-00-00_LTN - VAR - 2020-05-20 - 2021-06-21_results.xlsx"
            ), Times.Once());
        }
    }
}
