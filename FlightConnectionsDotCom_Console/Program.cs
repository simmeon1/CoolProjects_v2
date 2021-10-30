using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Console
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

            Logger_Console logger = new();
            ChromeDriver driver = null;
            if (parameters.LocalAirportListFile.IsNullOrEmpty() ||
                parameters.LocalAirportDestinationsFile.IsNullOrEmpty() ||
                (parameters.OpenGoogleFlights && parameters.LocalChromeWorkerResultsFile.IsNullOrEmpty())
                )
            {
                if (parameters.Headless)
                {
                    ChromeOptions chromeOptions = new();
                    chromeOptions.AddArgument("headless");
                    chromeOptions.AddArgument("window-size=1280,800");
                    driver = new(chromeOptions);
                }
                else driver = new();
            }

            RealWebDriverWait webDriverWait = new(driver);
            FlightConnectionsDotComWorker worker = new(logger, driver, webDriverWait);
            RealDelayer delayer = new();
            FullRunner runner = new(
                logger: logger,
                delayer: delayer,
                fileIO: new RealFileIO(),
                dateTimeProvider: new RealDateTimeProvider(),
                printer: new ExcelPrinter(),
                airportCollector: new FlightConnectionsDotComWorker_AirportCollector(worker),
                airportPopulator: new FlightConnectionsDotComWorker_AirportPopulator(worker),
                chromeWorker: new ChromeWorker(logger, delayer, driver)
            );
            bool success = await runner.DoRun(parameters);
            if ((success || parameters.Headless) && driver != null) driver.Quit();
            Console.WriteLine("Run finished. Press any key to continue");
            Console.ReadKey();
        }
    }
}
