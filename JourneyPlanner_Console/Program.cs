using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JourneyPlanner_Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = System.IO.File.ReadAllText(parametersPath).DeserializeObject<Parameters>();

            ChromeDriver driver = null;
            if (parameters.AirportListFile.IsNullOrEmpty() ||
                parameters.AirportDestinationsFile.IsNullOrEmpty() ||
                !parameters.OnlyPrintPaths
                )
            {
                ChromeOptions chromeOptions = new();
                chromeOptions.AddArgument("--log-level=3");
                if (parameters.Headless)
                {
                    chromeOptions.AddArgument("headless");
                    chromeOptions.AddArgument("window-size=1280,800");
                }
                driver = new(chromeOptions);
            }

            Logger_Console logger = new();
            RealWebDriverWait webDriverWait = new(driver);
            FlightConnectionsDotComWorker worker = new(logger, driver, webDriverWait);
            RealDelayer delayer = new();

            MultiJourneyCollector multiJourneyCollector = new(new JourneyRetrieverInstanceCreator());
            JourneyRetrieverComponents components = new(
                multiJourneyCollector,
                driver,
                logger,
                delayer,
                parameters.DefaultDelay
            );

            FullRunner runner = new(
                components,
                fileIO: new RealFileIO(),
                dateTimeProvider: new RealDateTimeProvider(),
                printer: new ExcelPrinter(),
                airportCollector: new FlightConnectionsDotComWorker_AirportCollector(worker),
                airportPopulator: new FlightConnectionsDotComWorker_AirportPopulator(worker),
                multiJourneyCollector);

            bool success = false;
            try
            {
                await runner.DoRun(parameters);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption appeared during run. Details:");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if ((success || parameters.Headless) && driver != null) driver.Quit();
                Console.WriteLine("Run finished. Press any key to continue");
                Console.ReadKey();
            }
        }
    }
}
